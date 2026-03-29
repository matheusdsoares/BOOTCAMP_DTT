using Microsoft.EntityFrameworkCore;
using SuaApi.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Configura o Banco de Dados (Voltamos ao simples para usar o mapeamento manual do DbContext)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Adiciona suporte a Controllers + Configuração de JSON para Enums
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// 3. Swagger/OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// 4. Mapeia as rotas das Controllers
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.Run();