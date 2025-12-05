using System.ComponentModel.DataAnnotations;

namespace SistemaEscolar.API.Models
{
    public class Padre
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;
        
        [Phone]
        [StringLength(15)]
        public string? Telefono { get; set; }
        
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(150)]
        public string? Email { get; set; }
        
        // true = Papá, false = Mamá
        public bool EsPadre { get; set; }
        
        // Nombre completo (calculado, no se guarda en BD)
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}