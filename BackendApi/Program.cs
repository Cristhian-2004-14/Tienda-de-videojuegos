using BackendApi.Data;
using BackendApi.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient("Firestore");
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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("FrontendVue");
app.UseAuthorization();
app.MapControllers();

app.Run();
