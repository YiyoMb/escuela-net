using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEscolar.API.Data;
using SistemaEscolar.API.Models;

namespace SistemaEscolar.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PadresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PadresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/padres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Padre>>> GetPadres()
        {
            var padres = await _context.Padres
                .OrderBy(p => p.Apellido)
                .ThenBy(p => p.Nombre)
                .ToListAsync();
            
            return Ok(padres);
        }

        // GET: api/padres/papas
        [HttpGet("papas")]
        public async Task<ActionResult<IEnumerable<Padre>>> GetPapas()
        {
            var papas = await _context.Padres
                .Where(p => p.EsPadre == true)
                .OrderBy(p => p.Apellido)
                .ThenBy(p => p.Nombre)
                .ToListAsync();
            
            return Ok(papas);
        }

        // GET: api/padres/mamas
        [HttpGet("mamas")]
        public async Task<ActionResult<IEnumerable<Padre>>> GetMamas()
        {
            var mamas = await _context.Padres
                .Where(p => p.EsPadre == false)
                .OrderBy(p => p.Apellido)
                .ThenBy(p => p.Nombre)
                .ToListAsync();
            
            return Ok(mamas);
        }

        // GET: api/padres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Padre>> GetPadre(int id)
        {
            var padre = await _context.Padres.FindAsync(id);

            if (padre == null)
            {
                return NotFound(new { message = $"Padre/Madre con ID {id} no encontrado(a)" });
            }

            return Ok(padre);
        }

        // POST: api/padres
        [HttpPost]
        public async Task<ActionResult<Padre>> CreatePadre(Padre padre)
        {
            // Validar que no exista el mismo email
            if (!string.IsNullOrEmpty(padre.Email))
            {
                var emailExiste = await _context.Padres
                    .AnyAsync(p => p.Email.ToLower() == padre.Email.ToLower());

                if (emailExiste)
                {
                    return BadRequest(new { message = "Ya existe un registro con ese email" });
                }
            }

            _context.Padres.Add(padre);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetPadre), 
                new { id = padre.Id }, 
                padre
            );
        }

        // PUT: api/padres/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePadre(int id, Padre padre)
        {
            if (id != padre.Id)
            {
                return BadRequest(new { message = "El ID no coincide" });
            }

            // Verificar que existe
            var existe = await _context.Padres.AnyAsync(p => p.Id == id);
            if (!existe)
            {
                return NotFound(new { message = $"Padre/Madre con ID {id} no encontrado(a)" });
            }

            // Validar email único (excluyendo el registro actual)
            if (!string.IsNullOrEmpty(padre.Email))
            {
                var emailExiste = await _context.Padres
                    .AnyAsync(p => p.Email.ToLower() == padre.Email.ToLower() && p.Id != id);

                if (emailExiste)
                {
                    return BadRequest(new { message = "Ya existe otro registro con ese email" });
                }
            }

            _context.Entry(padre).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, new { message = "Error al actualizar" });
            }

            return NoContent();
        }

        // DELETE: api/padres/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePadre(int id)
        {
            var padre = await _context.Padres.FindAsync(id);

            if (padre == null)
            {
                return NotFound(new { message = $"Padre/Madre con ID {id} no encontrado(a)" });
            }

            // Verificar si tiene hijos registrados como padre
            var tieneHijosPadre = await _context.Alumnos
                .AnyAsync(a => a.PadreId == id);

            // Verificar si tiene hijos registrados como madre
            var tieneHijosMadre = await _context.Alumnos
                .AnyAsync(a => a.MadreId == id);

            if (tieneHijosPadre || tieneHijosMadre)
            {
                return BadRequest(new { 
                    message = "No se puede eliminar porque tiene alumnos registrados como sus hijos" 
                });
            }

            _context.Padres.Remove(padre);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}