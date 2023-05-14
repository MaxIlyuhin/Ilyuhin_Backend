using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ilyuhin_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using MySqlX.XDevAPI;

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
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin, barber")]
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


        [HttpGet("BarbersScheduleByDate/{barberId}/{year}/{month}/{day}")]
        [Authorize(Roles = "admin, barber")]
        public async Task<ActionResult<IEnumerable<Client>>> BarbersScheduleByDate(int barberId, int year, int month, int day)
        {
            var bookings_by_date = _context.Bookings
                .Where(b => b.Id_of_Barber == barberId && b.Time_of_booking.Year == year && b.Time_of_booking.Month == month && b.Time_of_booking.Day == day)
                .Join(_context.Barber, b => b.Id_of_Barber, c => c.Id, (b, c) => new { Booking = b, Barber = c })
                .Select(x => new {
                    BarberName = x.Barber.FirstName,
                    Service = x.Booking.Service,
                    Price = x.Booking.Price,
                    Time_of_booking = x.Booking.Time_of_booking
                })
                .ToList();
            if (bookings_by_date == null)
            {
                return NotFound();
            }
            return Ok(bookings_by_date);
        }


        [HttpGet("TopFiveBarbers")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Client>>> GetTopFiveBarbers()
        {
               var topBarbers = await _context.Barber
              .Join(
                  _context.Bookings,
                  barber => barber.Id,
                  booking => booking.Id_of_Barber,
                  (barber, booking) => new { Barber = barber, Booking = booking }
              )
              .GroupBy(
                  x => x.Barber,
                  (barber, bookings) => new { BarberName = barber.FirstName, BookingCount = bookings.Count() }
              )
              .OrderByDescending(x => x.BookingCount)
              .Take(5)
              .ToListAsync();

            if (topBarbers == null)
            {
                return NotFound();
            }
            return Ok(topBarbers);
        }

        [HttpGet("TopThreeBarbersByProfit")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<Client>>> TopThreeBarbersByProfit()
        {

                 var topBarbers = _context.Barber
                     .Join(_context.Bookings,
                     barber => barber.Id,
                     booking => booking.Id_of_Barber,
                     (barber, booking) => new { Barber = barber, Booking = booking })
                    .GroupBy(x => new { x.Barber.Id, x.Barber.FirstName })
                    .Select(x => new { x.Key.Id, x.Key.FirstName, TotalEarnings = x.Sum(y => y.Booking.Price) })
                    .OrderByDescending(x => x.TotalEarnings)
                    .Take(3)
                    .ToList();

            if (topBarbers == null)
            {
                return NotFound();
            }
            return Ok(topBarbers);
        }

        // PUT: api/Barbers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "admin, barber")]
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
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin, barber")]
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
