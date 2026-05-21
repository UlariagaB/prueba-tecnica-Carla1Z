using Microsoft.EntityFrameworkCore;
using Prueba_tecnica.Contexto;
using Prueba_tecnica.Servicio;
using System;

var builder = WebApplication.CreateBuilder(args);

// Configurar In-Memory Database
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("WmsTestDb"));

// Inyección de dependencias para el servicio de logística y HttpClient
builder.Services.AddScoped<IRecepcionService, RecepcionService>();
builder.Services.AddHttpClient();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Crear el scope para asegurar que la DB en memoria se crea y se ejecute el Seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();