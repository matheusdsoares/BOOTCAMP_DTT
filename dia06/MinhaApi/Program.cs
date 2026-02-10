using Microsoft.EntityFrameworkCore;
using MinhaApi.Data; 
using System.Diagnostics.CodeAnalysis; // 1. O using fica no topo

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Registro do DbContext com Npgsql
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var cs = "Host=localhost;Port=5432;Database=minhaapi_db;Username=postgres;Password=postgres"; 
    options
        .UseNpgsql(cs)
        .UseSnakeCaseNamingConvention();
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

// 2. A classe partial com o atributo de exclusão fica aqui embaixo, fora do fluxo principal
[ExcludeFromCodeCoverage]
public partial class Program { }