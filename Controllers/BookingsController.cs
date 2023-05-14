using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ilyuhin_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using MySqlX.XDevAPI;

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


        [HttpGet("BookingsByDate/{year}/{month}/{day}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Client>>> BookingsByDate(int year, int month, int day)
        {
            //var bookings = _context.Bookings
            //    .Where(b => b.Time_of_booking.Year == year && b.Time_of_booking.Month == month && b.Time_of_booking.Day == day)
            //    .ToList();
            var bookings = from b in _context.Bookings
                           join c in _context.Customer on b.Id_of_customer equals c.Id
                           join r in _context.Barber on b.Id_of_Barber equals r.Id
                           where b.Time_of_booking.Year == year && b.Time_of_booking.Month == month && b.Time_of_booking.Day == day
                           select new
                           {
                               BookingId = b.Id,
                               Customer = new
                               {
                                   Id = c.Id,
                                   FirstName = c.FirstName,
                                   LastName = c.LastName,
                                   Telephone = c.Telephone,
                                   Email = c.Email
                               },
                               Barber = new
                               {
                                   Id = r.Id,
                                   FirstName = r.FirstName,
                                   Rating = r.Rating
                               },
                               Service = b.Service,
                               Price = b.Price,
                               TimeOfBooking = b.Time_of_booking
                           };

            if (bookings == null)
            {
                return NotFound();
            }
            return Ok(bookings);
        }


        [HttpGet("TheMostPopularServices")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Client>>> TheMostPopularServices()
        {
            var popularServices = await _context.Bookings
                          .GroupBy(b => b.Service)
                          .OrderByDescending(g => g.Count())
                          .Select(g => new { Service = g.Key, Count = g.Count() })
                          .ToListAsync();
            if (popularServices == null)
            {
                return NotFound();
            }
            return Ok(popularServices);
        }


        [HttpGet("TheMostPopularDate")]
        [Authorize(Roles = "admin, barber")]
        public async Task<ActionResult<IEnumerable<Client>>> TheMostPopularDate()
        {
            var mostPopularDate = await _context.Bookings
                .GroupBy(b => b.Time_of_booking.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Select(x => x.Date.ToString("dddd"))
                .FirstOrDefaultAsync();

            if (mostPopularDate == null)
            {
                return NotFound();
            }
            return Ok(mostPopularDate);
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
        [Authorize]
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
