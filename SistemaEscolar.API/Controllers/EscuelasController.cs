using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEscolar.API.Data;
using SistemaEscolar.API.Models;

namespace SistemaEscolar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EscuelasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EscuelasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/escuelas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Escuela>>> GetEscuelas()
        {
            var escuelas = await _context.Escuelas
                .OrderBy(e => e.Nombre)
                .ToListAsync();
            
            return Ok(escuelas);
        }

        // GET: api/escuelas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Escuela>> GetEscuela(int id)
        {
            var escuela = await _context.Escuelas.FindAsync(id);

            if (escuela == null)
            {
                return NotFound(new { message = $"Escuela con ID {id} no encontrada" });
            }

            return Ok(escuela);
        }

        // POST: api/escuelas
        [HttpPost]
        public async Task<ActionResult<Escuela>> CreateEscuela(Escuela escuela)
        {
            // Validar que el nombre no esté duplicado
            var existe = await _context.Escuelas
                .AnyAsync(e => e.Nombre.ToLower() == escuela.Nombre.ToLower());

            if (existe)
            {
                return BadRequest(new { message = "Ya existe una escuela con ese nombre" });
            }

            _context.Escuelas.Add(escuela);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEscuela), 
                new { id = escuela.Id }, 
                escuela
            );
        }

        // PUT: api/escuelas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEscuela(int id, Escuela escuela)
        {
            if (id != escuela.Id)
            {
                return BadRequest(new { message = "El ID no coincide" });
            }

            // Verificar que existe
            var existe = await _context.Escuelas.AnyAsync(e => e.Id == id);
            if (!existe)
            {
                return NotFound(new { message = $"Escuela con ID {id} no encontrada" });
            }

            _context.Entry(escuela).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "Error al actualizar la escuela" });
            }

            return NoContent();
        }

        // DELETE: api/escuelas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEscuela(int id)
        {
            var escuela = await _context.Escuelas
                .Include(e => e.Alumnos)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (escuela == null)
            {
                return NotFound(new { message = $"Escuela con ID {id} no encontrada" });
            }

            // Validar que no tenga alumnos
            if (escuela.Alumnos.Any())
            {
                return BadRequest(new { 
                    message = "No se puede eliminar una escuela que tiene alumnos registrados",
                    alumnosCount = escuela.Alumnos.Count
                });
            }

            _context.Escuelas.Remove(escuela);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}