using Microsoft.EntityFrameworkCore;
using SistemaEscolar.API.Models;

namespace SistemaEscolar.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        // DbSets (Tablas)
        public DbSet<Escuela> Escuelas { get; set; }
        public DbSet<Padre> Padres { get; set; }
        public DbSet<Alumno> Alumnos { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuración de Escuela
            modelBuilder.Entity<Escuela>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Direccion).HasMaxLength(300);
                entity.Property(e => e.Telefono).HasMaxLength(15);
            });
            
            // Configuración de Padre
            modelBuilder.Entity<Padre>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Apellido).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Telefono).HasMaxLength(15);
                entity.Property(p => p.Email).HasMaxLength(150);
                entity.Property(p => p.EsPadre).IsRequired();
                
                // Índice para búsquedas rápidas por género
                entity.HasIndex(p => p.EsPadre);
            });
            
            // Configuración de Alumno
            modelBuilder.Entity<Alumno>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(a => a.Apellido).IsRequired().HasMaxLength(100);
                entity.Property(a => a.FechaNacimiento).IsRequired();
                entity.Property(a => a.Grado).HasMaxLength(50);
                
                // Relación con Padre (Papá)
                entity.HasOne(a => a.Padre)
                    .WithMany()
                    .HasForeignKey(a => a.PadreId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Relación con Madre (Mamá)
                entity.HasOne(a => a.Madre)
                    .WithMany()
                    .HasForeignKey(a => a.MadreId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                // Relación con Escuela
                entity.HasOne(a => a.Escuela)
                    .WithMany(e => e.Alumnos)
                    .HasForeignKey(a => a.EscuelaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            // Datos iniciales de ejemplo
            SeedData(modelBuilder);
        }
        
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Escuelas de ejemplo
            modelBuilder.Entity<Escuela>().HasData(
                new Escuela 
                { 
                    Id = 1, 
                    Nombre = "Escuela Primaria Benito Juárez", 
                    Direccion = "Av. Principal 123", 
                    Telefono = "4491234567" 
                },
                new Escuela 
                { 
                    Id = 2, 
                    Nombre = "Instituto Miguel Hidalgo", 
                    Direccion = "Calle Reforma 456", 
                    Telefono = "4499876543" 
                }
            );
            
            // Padres de ejemplo
            modelBuilder.Entity<Padre>().HasData(
                // Papás
                new Padre 
                { 
                    Id = 1, 
                    Nombre = "Juan", 
                    Apellido = "García", 
                    Telefono = "4491111111", 
                    Email = "juan.garcia@email.com", 
                    EsPadre = true 
                },
                new Padre 
                { 
                    Id = 3, 
                    Nombre = "Pedro", 
                    Apellido = "López", 
                    Telefono = "4493333333", 
                    Email = "pedro.lopez@email.com", 
                    EsPadre = true 
                },
                // Mamás
                new Padre 
                { 
                    Id = 2, 
                    Nombre = "María", 
                    Apellido = "Rodríguez", 
                    Telefono = "4492222222", 
                    Email = "maria.rodriguez@email.com", 
                    EsPadre = false 
                },
                new Padre 
                { 
                    Id = 4, 
                    Nombre = "Ana", 
                    Apellido = "Martínez", 
                    Telefono = "4494444444", 
                    Email = "ana.martinez@email.com", 
                    EsPadre = false 
                }
            );
            
            // Alumnos de ejemplo
            modelBuilder.Entity<Alumno>().HasData(
                new Alumno 
                { 
                    Id = 1, 
                    Nombre = "Carlos", 
                    Apellido = "García Rodríguez", 
                    FechaNacimiento = new DateTime(2015, 5, 10),
                    Grado = "3° Primaria",
                    PadreId = 1,
                    MadreId = 2,
                    EscuelaId = 1
                },
                new Alumno 
                { 
                    Id = 2, 
                    Nombre = "Laura", 
                    Apellido = "López Martínez", 
                    FechaNacimiento = new DateTime(2016, 8, 22),
                    Grado = "2° Primaria",
                    PadreId = 3,
                    MadreId = 4,
                    EscuelaId = 1
                }
            );
        }
    }
}