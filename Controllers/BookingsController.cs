using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ilyuhin_Backend.Models;
using Microsoft.AspNetCore.Authorization;

namespace Ilyuhin_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly LibContext _context;

        public BookingsController(LibContext context)
        {
            _context = context;
        }

        // GET: api/Bookings
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Bookings>>> GetBookings()
        {
          if (_context.Bookings == null)
          {
              return NotFound();
          }
            return await _context.Bookings.ToListAsync();
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin, barber")]
        public async Task<ActionResult<Bookings>> GetBookings(int id)
        {
          if (_context.Bookings == null)
          {
              return NotFound();
          }
            var bookings = await _context.Bookings.FindAsync(id);

            if (bookings == null)
            {
                return NotFound();
            }

            return bookings;
        }

        // PUT: api/Bookings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutBookings(int id, Bookings bookings)
        {
            if (id != bookings.Id)
            {
                return BadRequest();
            }

            _context.Entry(bookings).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingsExists(id))
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

        // POST: api/Bookings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Bookings>> PostBookings(Bookings bookings)
        {
          if (_context.Bookings == null)
          {
              return Problem("Entity set 'LibContext.Bookings'  is null.");
          }
            _context.Bookings.Add(bookings);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBookings", new { id = bookings.Id }, bookings);
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBookings(int id)
        {
            if (_context.Bookings == null)
            {
                return NotFound();
            }
            var bookings = await _context.Bookings.FindAsync(id);
            if (bookings == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(bookings);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookingsExists(int id)
        {
            return (_context.Bookings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
