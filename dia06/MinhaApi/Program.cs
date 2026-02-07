using Microsoft.EntityFrameworkCore;
using MinhaApi.Data; 

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


// Recomendação do Npgsql para compatibilidade de timestamp (se aplicável)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Mapear controllers
app.MapControllers();

app.Run();