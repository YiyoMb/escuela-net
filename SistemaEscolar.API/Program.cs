using System.IO;
using Microsoft.EntityFrameworkCore;
using SistemaEscolar.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor ignorando referencias circulares
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Configurar base de datos (SQLite para producción con baja RAM, SQL Server para desarrollo)
var useSqlite = builder.Configuration.GetValue<bool>("UseSqlite", false);

if (useSqlite)
{
    // Crear directorio data si no existe
    var dbPath = Path.Combine(builder.Environment.ContentRootPath, "data", "escolar.db");
    var directory = Path.GetDirectoryName(dbPath);
    if (!Directory.Exists(directory))
    {
        Directory.CreateDirectory(directory!);
    }
    
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite($"Data Source={dbPath}"));
    
    Console.WriteLine($"✅ [DEPLOY AUTO] Usando SQLite: {dbPath}");
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    
    Console.WriteLine("✅ [DEPLOY AUTO] Usando SQL Server");
}

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Kestrel (solo para Docker o cuando lo necesites)
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8080);
});

var app = builder.Build();

// ===== AGREGAR ESTE BLOQUE COMPLETO =====
// Aplicar migraciones automáticamente
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        Console.WriteLine("✅ Migraciones aplicadas correctamente");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al aplicar migraciones: {ex.Message}");
    }
}

// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Archivos estáticos
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Necesario para los tests de integración
public partial class Program { }
