using Microsoft.EntityFrameworkCore;
using MinhaApi.Data; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
<<<<<<< HEAD

// Registro do DbContext com Npgsql

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection"); 
    options
        .UseNpgsql(cs);
       // .UseSnakeCaseNamingConvention();
});


// Recomendação do Npgsql para compatibilidade de timestamp (se aplicável)
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

=======
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
>>>>>>> 2798c70ea5148c100fd819809d9ab9acdf6ed2ee
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

<<<<<<< HEAD
// Mapear controllers
app.MapControllers();

app.Run();
=======
app.MapControllers();

app.Run();
>>>>>>> 2798c70ea5148c100fd819809d9ab9acdf6ed2ee
