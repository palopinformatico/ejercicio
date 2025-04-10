using Microsoft.EntityFrameworkCore;
using ClientesApi.Data; // Asegúrate de que la ruta sea correcta para tu contexto

var builder = WebApplication.CreateBuilder(args);

// Configura CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:4200")  // Dirección de tu app Angular
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Agrega el servicio DbContext con la configuración de SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar soporte para controladores
builder.Services.AddControllers();

var app = builder.Build();

// Usa la política CORS que hemos configurado
app.UseCors("AllowLocalhost");

// Resto de la configuración
app.MapControllers();

app.Run();
