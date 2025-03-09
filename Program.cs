using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Api_Godot_PSP.MongoDb;
using Api_Godot_PSP.Services;
var builder = WebApplication.CreateBuilder(args);

// Configurar servicios de MongoDB
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<MongoDbService>();

// Configurar controladores y opciones de serialización JSON
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Configurar otros servicios necesarios aquí
// builder.Services.Add... (si agregas más servicios en el futuro)

var app = builder.Build();

// Configuración de Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Mapeo de controladores (este es el que conecta las rutas con los controladores)
app.MapControllers();

// Ejecutar la aplicación
app.Run();