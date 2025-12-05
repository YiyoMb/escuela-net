using System.ComponentModel.DataAnnotations;

namespace SistemaEscolar.API.Models
{
    public class Escuela
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre de la escuela es obligatorio")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Nombre { get; set; } = string.Empty;
        
        [StringLength(300)]
        public string? Direccion { get; set; }
        
        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        [StringLength(15)]
        public string? Telefono { get; set; }
        
        // Relación: Una escuela tiene muchos alumnos
        public List<Alumno> Alumnos { get; set; } = new();
    }
}