using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using BackendApi.Models;

namespace BackendApi.Data;

public class FirestoreRepository<T> : IFirestoreRepository<T>
    where T : class, IEntidad
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient _http;
    private readonly string _coleccion;
    private readonly string _baseUrl;

    public FirestoreRepository(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        var projectId = configuration["Firebase:ProjectId"]
            ?? throw new InvalidOperationException("Falta Firebase:ProjectId.");
        var apiKey = configuration["Firebase:ApiKey"]
            ?? throw new InvalidOperationException("Falta Firebase:ApiKey.");

        _http = httpClientFactory.CreateClient("Firestore");
        _coleccion = ObtenerNombreColeccion();
        _baseUrl =
            $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents";
        ApiKey = apiKey;
    }

    private string ApiKey { get; }

    public async Task<IReadOnlyList<T>> ObtenerTodosAsync()
    {
        using var response = await _http.GetAsync($"{_baseUrl}/{_coleccion}?key={ApiKey}");
        response.EnsureSuccessStatusCode();
        var root = await LeerJsonAsync(response);

        if (!root.TryGetProperty("documents", out var documents)) return [];

        return documents.EnumerateArray()
            .Select(ConvertirDocumento)
            .OrderBy(entidad => entidad.Id)
            .ToList();
    }

    public async Task<T?> ObtenerPorIdAsync(int id)
    {
        using var response = await _http.GetAsync($"{_baseUrl}/{_coleccion}/{id}?key={ApiKey}");
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return ConvertirDocumento(await LeerJsonAsync(response));
    }

    public async Task<T> CrearAsync(T entidad)
    {
        var existentes = await ObtenerTodosAsync();
        entidad.Id = existentes.Count == 0 ? 1 : existentes.Max(item => item.Id) + 1;
        await GuardarAsync(entidad.Id, entidad);
        return entidad;
    }

    public async Task<T?> ActualizarAsync(int id, T entidad)
    {
        if (await ObtenerPorIdAsync(id) is null) return null;
        entidad.Id = id;
        await GuardarAsync(id, entidad);
        return entidad;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        if (await ObtenerPorIdAsync(id) is null) return false;
        using var response = await _http.DeleteAsync($"{_baseUrl}/{_coleccion}/{id}?key={ApiKey}");
        response.EnsureSuccessStatusCode();
        return true;
    }

    private async Task GuardarAsync(int id, T entidad)
    {
        var elemento = JsonSerializer.SerializeToElement(entidad, JsonOptions);
        var fields = elemento.EnumerateObject()
            .ToDictionary(propiedad => propiedad.Name, propiedad => CrearValorFirestore(propiedad.Value));
        var contenido = JsonSerializer.Serialize(new { fields });
        using var request = new HttpRequestMessage(
            HttpMethod.Patch,
            $"{_baseUrl}/{_coleccion}/{id}?key={ApiKey}")
        {
            Content = new StringContent(contenido, Encoding.UTF8, "application/json"),
        };
        using var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    private static object CrearValorFirestore(JsonElement valor) => valor.ValueKind switch
    {
        JsonValueKind.String when valor.TryGetDateTime(out var fecha) =>
            new { timestampValue = fecha.ToUniversalTime().ToString("O") },
        JsonValueKind.String => new { stringValue = valor.GetString() },
        JsonValueKind.Number when valor.TryGetInt64(out var entero) =>
            new { integerValue = entero.ToString(CultureInfo.InvariantCulture) },
        JsonValueKind.Number => new { doubleValue = valor.GetDouble() },
        JsonValueKind.True => new { booleanValue = true },
        JsonValueKind.False => new { booleanValue = false },
        JsonValueKind.Null => new { nullValue = "NULL_VALUE" },
        JsonValueKind.Array => new
        {
            arrayValue = new
            {
                values = valor.EnumerateArray().Select(CrearValorFirestore).ToArray(),
            },
        },
        JsonValueKind.Object => new
        {
            mapValue = new
            {
                fields = valor.EnumerateObject()
                    .ToDictionary(propiedad => propiedad.Name, propiedad => CrearValorFirestore(propiedad.Value)),
            },
        },
        _ => throw new InvalidOperationException("Tipo de dato no compatible con Firestore."),
    };

    private static T ConvertirDocumento(JsonElement documento)
    {
        var fields = documento.GetProperty("fields");
        var json = new JsonObject();
        foreach (var field in fields.EnumerateObject())
        {
            json[field.Name] = LeerValorFirestore(field.Value);
        }

        return json.Deserialize<T>(JsonOptions)
            ?? throw new InvalidOperationException($"No se pudo convertir un documento a {typeof(T).Name}.");
    }

    private static JsonNode? LeerValorFirestore(JsonElement valor)
    {
        if (valor.TryGetProperty("stringValue", out var texto)) return JsonValue.Create(texto.GetString());
        if (valor.TryGetProperty("integerValue", out var entero)) return JsonValue.Create(long.Parse(entero.GetString()!));
        if (valor.TryGetProperty("doubleValue", out var doble)) return JsonValue.Create(doble.GetDouble());
        if (valor.TryGetProperty("booleanValue", out var booleano)) return JsonValue.Create(booleano.GetBoolean());
        if (valor.TryGetProperty("timestampValue", out var fecha)) return JsonValue.Create(fecha.GetString());
        if (valor.TryGetProperty("nullValue", out _)) return null;
        if (valor.TryGetProperty("arrayValue", out var array))
        {
            var resultado = new JsonArray();
            if (array.TryGetProperty("values", out var values))
                foreach (var item in values.EnumerateArray()) resultado.Add(LeerValorFirestore(item));
            return resultado;
        }
        if (valor.TryGetProperty("mapValue", out var map))
        {
            var resultado = new JsonObject();
            if (map.TryGetProperty("fields", out var mapFields))
                foreach (var field in mapFields.EnumerateObject())
                    resultado[field.Name] = LeerValorFirestore(field.Value);
            return resultado;
        }
        throw new InvalidOperationException("Valor de Firestore desconocido.");
    }

    private static async Task<JsonElement> LeerJsonAsync(HttpResponseMessage response)
    {
        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);
        return document.RootElement.Clone();
    }

    private static string ObtenerNombreColeccion() => typeof(T).Name switch
    {
        nameof(Cliente) => "clientes",
        nameof(Producto) => "productos",
        nameof(Venta) => "ventas",
        nameof(Servicio) => "servicios",
        nameof(Usuario) => "usuarios",
        nameof(Empleado) => "empleados",
        nameof(Rol) => "roles",
        nameof(Proveedor) => "proveedores",
        nameof(Compra) => "compras",
        nameof(Dispositivo) => "dispositivos",
        _ => throw new InvalidOperationException($"No existe colección para {typeof(T).Name}."),
    };
}
