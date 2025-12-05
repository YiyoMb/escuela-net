using Microsoft.EntityFrameworkCore;
using SistemaEscolar.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
//builder.Services.AddControllers();

//Agregar servicios al contenedor ignorando las referencias circulares
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = 
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Solo registrar SQL Server si NO estamos en entorno de pruebas
if (builder.Environment.EnvironmentName != "Testing")
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    );
}

// Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar CORS (permitir peticiones desde el frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Configurar para escuchar en el puerto correcto dentro del contenedor
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8080);
});

var app = builder.Build();

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// IMPORTANTE: Habilitar archivos estáticos (HTML, CSS, JS)
app.UseDefaultFiles(); // Busca index.html por defecto
app.UseStaticFiles();  // Sirve archivos de wwwroot

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Hacer la clase Program accesible para pruebas de integración
public partial class Program { }