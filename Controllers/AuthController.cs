using ComputerCourses.Models;
using Google.Protobuf.WellKnownTypes;
using Ilyuhin_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
using System.Security.Claims;

namespace Ilyuhin_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LibContext _context;
        public AuthController(LibContext context)
        {
            _context = context;
        }
        public struct LoginData
        {
            public string login { get; set; }
            public string password { get; set; }
        }

        [HttpPost("Authorization")]
        public object GetCustomerToken([FromBody] LoginData ld)
        {
            var user = _context.Customer.FirstOrDefault(u => u.Username == ld.login && u.Password == ld.password);
            var barber = _context.Barber.FirstOrDefault(u => u.Username == ld.login && u.Password == ld.password);
            if (user == null && barber == null)
            {
                Response.StatusCode = 401;
                return new { message = "wrong login/password" };
            }

            var role = user != null ? user.Role : barber.Role;
            if (user != null)
            {
                    return new { token = AuthOptions.GenerateToken(user.Role), message = user.WelcomeMessage() };
                }
            else
            {
                    return new { token = AuthOptions.GenerateToken(barber.Role), message = barber.WelcomeMessage() };
                }
        }


        private bool CustomerExists(int id)
        {
            return _context.Customer.Any(e => e.Id == id);
        }

        private bool BarberExists(int id)
        {
            return _context.Barber.Any(e => e.Id == id);
        }

        [HttpPut("ChangeUsername/{id}")]
        [Authorize]
        public async Task<ActionResult<object>> ChangeUsername(int id, [FromForm] string new_username)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "guest")
            {
                var customer = await _context.Customer.FindAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }
                var usernameExists = await _context.Customer.AnyAsync(c => c.Username == new_username);
                if (usernameExists)
                {
                    return BadRequest("Username already exists");
                }
                customer.Username = new_username;
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
                return customer;
            }

            if (role == "barber")
            {
                var barber = await _context.Barber.FindAsync(id);
                if (barber == null)
                {
                    return NotFound();
                }
                var usernameExists = await _context.Barber.AnyAsync(c => c.Username == new_username);
                if (usernameExists)
                {
                    return BadRequest("Username already exists");
                }
                barber.Username = new_username;
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
                return barber;
            }
            else { return BadRequest("Unknown Authorization role"); }
        }

        [HttpPut("ChangePassword/{id}")]
        [Authorize]
        public async Task<ActionResult<object>> ChangePassword(int id, [FromForm] string new_password)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "guest")
            {
                var customer = await _context.Customer.FindAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }
                customer.Password = new_password;
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
                return customer;
            }

            if (role == "barber")
            {
                var barber = await _context.Barber.FindAsync(id);
                if (barber == null)
                {
                    return NotFound();
                }
                barber.Password = new_password;
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
                return barber;
            }
            else { return BadRequest("Unknown Authorization role"); }
        }
    }
}
