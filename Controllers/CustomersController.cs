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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ilyuhin_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly LibContext _context;

        public CustomersController(LibContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomer()
        {
          if (_context.Customer == null)
          {
              return NotFound();
          }
            return await _context.Customer.ToListAsync();
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
          if (_context.Customer == null)
          {
              return NotFound();
          }
            var customer = await _context.Customer.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }


        [HttpGet("CustomersNumberOfVisits")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<Client>>> CustomersNumberOfVisits()
        {
            var query = from customer in _context.Customer
                        join booking in _context.Bookings on customer.Id equals booking.Id_of_customer
                        group customer by customer.FirstName into g
                        select new
                        {
                            CustomerName = g.Key,
                            BookingsCount = g.Count()
                        };
            if (query == null)
            {
                return NotFound();
            }
            return Ok(query);
        }

        [HttpGet("PopularServicesByCustomer")]
        [Authorize(Roles = "admin, barber")]
        public async Task<ActionResult<IEnumerable<Client>>> PopularServicesByCustomers()
        {
            var popularServicesByCustomer = _context.Bookings
                .Join(_context.Customer, b => b.Id_of_customer, c => c.Id, (b, c) => new { Booking = b, Customer = c })
                .GroupBy(bc => new { bc.Customer.FirstName, bc.Customer.LastName })
                .Select(g => new
                {
                    Name = g.Key.FirstName + " " + g.Key.LastName,
                    PopularService = g.GroupBy(bc => bc.Booking.Service)
                                     .OrderByDescending(g2 => g2.Count())
                                     .Select(g2 => g2.Key)
                                     .FirstOrDefault()
                })
                .ToList();

            if (popularServicesByCustomer == null)
            {
                return NotFound();
            }
            return Ok(popularServicesByCustomer);
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
          if (_context.Customer == null)
          {
              return Problem("Entity set 'LibContext.Customer'  is null.");
          }
            _context.Customer.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (_context.Customer == null)
            {
                return NotFound();
            }
            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customer.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return (_context.Customer?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
