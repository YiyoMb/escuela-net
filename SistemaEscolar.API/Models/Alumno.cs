using System.ComponentModel.DataAnnotations;

namespace SistemaEscolar.API.Models
{
    public class Alumno
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        public DateTime FechaNacimiento { get; set; }
        
        [StringLength(50)]
        public string? Grado { get; set; }
        
        // Foreign Keys
        [Required(ErrorMessage = "El padre es obligatorio")]
        public int PadreId { get; set; }
        
        [Required(ErrorMessage = "La madre es obligatoria")]
        public int MadreId { get; set; }
        
        [Required(ErrorMessage = "La escuela es obligatoria")]
        public int EscuelaId { get; set; }
        
        // Propiedades de navegación
        public Padre? Padre { get; set; }
        public Padre? Madre { get; set; }
        public Escuela? Escuela { get; set; }
        
        // Nombre completo (calculado)
        public string NombreCompleto => $"{Nombre} {Apellido}";
        
        // Edad calculada
        public int Edad
        {
            get
            {
                var hoy = DateTime.Today;
                var edad = hoy.Year - FechaNacimiento.Year;
                if (FechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
                return edad;
            }
        }
    }
}