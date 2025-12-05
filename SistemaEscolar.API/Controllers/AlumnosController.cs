using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEscolar.API.Data;
using SistemaEscolar.API.Models;

namespace SistemaEscolar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlumnosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AlumnosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/alumnos
        // Listado simple sin relaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alumno>>> GetAlumnos()
        {
            var alumnos = await _context.Alumnos
                .OrderBy(a => a.Apellido)
                .ThenBy(a => a.Nombre)
                .ToListAsync();
            
            return Ok(alumnos);
        }

        // GET: api/alumnos/completo
        // ⭐ CONSULTA PRINCIPAL - Listado con mamá, papá y escuela
        [HttpGet("completo")]
        public async Task<ActionResult> GetAlumnosCompleto()
        {
            var alumnos = await _context.Alumnos
                .Include(a => a.Padre)    // Incluir información del papá
                .Include(a => a.Madre)    // Incluir información de la mamá
                .Include(a => a.Escuela)  // Incluir información de la escuela
                .OrderBy(a => a.Apellido)
                .ThenBy(a => a.Nombre)
                .Select(a => new
                {
                    // Datos del alumno
                    id = a.Id,
                    nombre = a.Nombre,
                    apellido = a.Apellido,
                    nombreCompleto = a.NombreCompleto,
                    fechaNacimiento = a.FechaNacimiento.ToString("yyyy-MM-dd"),
                    edad = a.Edad,
                    grado = a.Grado,
                    
                    // Datos del papá
                    padre = new
                    {
                        id = a.Padre!.Id,
                        nombreCompleto = a.Padre.NombreCompleto,
                        telefono = a.Padre.Telefono,
                        email = a.Padre.Email
                    },
                    
                    // Datos de la mamá
                    madre = new
                    {
                        id = a.Madre!.Id,
                        nombreCompleto = a.Madre.NombreCompleto,
                        telefono = a.Madre.Telefono,
                        email = a.Madre.Email
                    },
                    
                    // Datos de la escuela
                    escuela = new
                    {
                        id = a.Escuela!.Id,
                        nombre = a.Escuela.Nombre,
                        direccion = a.Escuela.Direccion,
                        telefono = a.Escuela.Telefono
                    }
                })
                .ToListAsync();

            return Ok(alumnos);
        }

        // GET: api/alumnos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Alumno>> GetAlumno(int id)
        {
            var alumno = await _context.Alumnos
                .Include(a => a.Padre)
                .Include(a => a.Madre)
                .Include(a => a.Escuela)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (alumno == null)
            {
                return NotFound(new { message = $"Alumno con ID {id} no encontrado" });
            }

            return Ok(alumno);
        }

        // POST: api/alumnos
        [HttpPost]
        public async Task<ActionResult<Alumno>> CreateAlumno(Alumno alumno)
        {
            // Validar que el padre existe y es hombre
            var padre = await _context.Padres.FindAsync(alumno.PadreId);
            if (padre == null)
            {
                return BadRequest(new { message = "El padre especificado no existe" });
            }
            if (padre.EsPadre == false)
            {
                return BadRequest(new { message = "El ID especificado como padre corresponde a una madre" });
            }

            // Validar que la madre existe y es mujer
            var madre = await _context.Padres.FindAsync(alumno.MadreId);
            if (madre == null)
            {
                return BadRequest(new { message = "La madre especificada no existe" });
            }
            if (madre.EsPadre == true)
            {
                return BadRequest(new { message = "El ID especificado como madre corresponde a un padre" });
            }

            // Validar que la escuela existe
            var escuela = await _context.Escuelas.FindAsync(alumno.EscuelaId);
            if (escuela == null)
            {
                return BadRequest(new { message = "La escuela especificada no existe" });
            }

            // Validar fecha de nacimiento (no puede ser futura)
            if (alumno.FechaNacimiento > DateTime.Today)
            {
                return BadRequest(new { message = "La fecha de nacimiento no puede ser futura" });
            }

            _context.Alumnos.Add(alumno);
            await _context.SaveChangesAsync();

            // Cargar las relaciones para la respuesta
            await _context.Entry(alumno).Reference(a => a.Padre).LoadAsync();
            await _context.Entry(alumno).Reference(a => a.Madre).LoadAsync();
            await _context.Entry(alumno).Reference(a => a.Escuela).LoadAsync();

            return CreatedAtAction(
                nameof(GetAlumno), 
                new { id = alumno.Id }, 
                alumno
            );
        }

        // PUT: api/alumnos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAlumno(int id, Alumno alumno)
        {
            if (id != alumno.Id)
            {
                return BadRequest(new { message = "El ID no coincide" });
            }

            // Verificar que existe
            var existe = await _context.Alumnos.AnyAsync(a => a.Id == id);
            if (!existe)
            {
                return NotFound(new { message = $"Alumno con ID {id} no encontrado" });
            }

            // Validar padre
            var padre = await _context.Padres.FindAsync(alumno.PadreId);
            if (padre == null || padre.EsPadre == false)
            {
                return BadRequest(new { message = "Padre inválido" });
            }

            // Validar madre
            var madre = await _context.Padres.FindAsync(alumno.MadreId);
            if (madre == null || madre.EsPadre == true)
            {
                return BadRequest(new { message = "Madre inválida" });
            }

            // Validar escuela
            var escuelaExiste = await _context.Escuelas.AnyAsync(e => e.Id == alumno.EscuelaId);
            if (!escuelaExiste)
            {
                return BadRequest(new { message = "Escuela inválida" });
            }

            // Validar fecha de nacimiento
            if (alumno.FechaNacimiento > DateTime.Today)
            {
                return BadRequest(new { message = "La fecha de nacimiento no puede ser futura" });
            }

            _context.Entry(alumno).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "Error al actualizar el alumno" });
            }

            return NoContent();
        }

        // DELETE: api/alumnos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlumno(int id)
        {
            var alumno = await _context.Alumnos.FindAsync(id);

            if (alumno == null)
            {
                return NotFound(new { message = $"Alumno con ID {id} no encontrado" });
            }

            _context.Alumnos.Remove(alumno);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/alumnos/escuela/1
        // Obtener alumnos por escuela
        [HttpGet("escuela/{escuelaId}")]
        public async Task<ActionResult> GetAlumnosPorEscuela(int escuelaId)
        {
            var escuela = await _context.Escuelas.FindAsync(escuelaId);
            if (escuela == null)
            {
                return NotFound(new { message = "Escuela no encontrada" });
            }

            var alumnos = await _context.Alumnos
                .Include(a => a.Padre)
                .Include(a => a.Madre)
                .Where(a => a.EscuelaId == escuelaId)
                .OrderBy(a => a.Apellido)
                .ThenBy(a => a.Nombre)
                .ToListAsync();

            return Ok(alumnos);
        }
    }
}