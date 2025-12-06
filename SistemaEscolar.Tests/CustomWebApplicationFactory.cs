using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SistemaEscolar.API.Data;

namespace SistemaEscolar.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // IMPORTANTÍSIMO: declarar que estamos en entorno "Testing"
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // 1️⃣ Localizar el DbContext registrado originalmente (SQL Server)
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)
                );

                // 2️⃣ Eliminarlo para evitar conflictos
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // 3️⃣ Agregar el DbContext con InMemory para pruebas
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });

                // 4️⃣ Construir proveedor y crear BD en memoria
                var serviceProvider = services.BuildServiceProvider();

                using var scope = serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
            });
        }
    }
}
