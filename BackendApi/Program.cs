using BackendApi.Data;
using BackendApi.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Evita que los procesos de desarrollo sin privilegios intenten escribir
// en el registro de eventos de Windows.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient("Firestore", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddScoped(typeof(IFirestoreRepository<>), typeof(FirestoreRepository<>));
builder.Services.AddSingleton<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendVue", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.Use(async (context, siguiente) =>
{
    try
    {
        await siguiente();
    }
    catch (HttpRequestException error)
    {
        app.Logger.LogError(error, "No se pudo completar una operación con Firestore.");
        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        await context.Response.WriteAsJsonAsync(new
        {
            message = "Firestore no está disponible. Verifica la conexión y la configuración de Firebase.",
        });
    }
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("FrontendVue");
app.UseAuthorization();
app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "BackendApi",
    timestamp = DateTimeOffset.UtcNow,
}));
app.MapControllers();

app.Run();
