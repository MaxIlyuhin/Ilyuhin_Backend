using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ilyuhin_Backend.Models;

namespace Ilyuhin_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BarbersController : ControllerBase
    {
        private readonly LibContext _context;

        public BarbersController(LibContext context)
        {
            _context = context;
        }

        // GET: api/Barbers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Barber>>> GetBarber()
        {
          if (_context.Barber == null)
          {
              return NotFound();
          }
            return await _context.Barber.ToListAsync();
        }

        // GET: api/Barbers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Barber>> GetBarber(int id)
        {
          if (_context.Barber == null)
          {
              return NotFound();
          }
            var barber = await _context.Barber.FindAsync(id);

            if (barber == null)
            {
                return NotFound();
            }

            return barber;
        }

        // PUT: api/Barbers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBarber(int id, Barber barber)
        {
            if (id != barber.Id)
            {
                return BadRequest();
            }

            _context.Entry(barber).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BarberExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Barbers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Barber>> PostBarber(Barber barber)
        {
          if (_context.Barber == null)
          {
              return Problem("Entity set 'LibContext.Barber'  is null.");
          }
            _context.Barber.Add(barber);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBarber", new { id = barber.Id }, barber);
        }

        // DELETE: api/Barbers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBarber(int id)
        {
            if (_context.Barber == null)
            {
                return NotFound();
            }
            var barber = await _context.Barber.FindAsync(id);
            if (barber == null)
            {
                return NotFound();
            }

            _context.Barber.Remove(barber);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BarberExists(int id)
        {
            return (_context.Barber?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
