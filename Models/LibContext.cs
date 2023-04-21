using Microsoft.EntityFrameworkCore;

namespace Ilyuhin_Backend.Models
{
    public class LibContext : DbContext
    {
        public LibContext(DbContextOptions<LibContext> options): 
            base(options)
        {
            Database.EnsureCreated();
            //Database.Migrate();
        }
        public DbSet<Barber> Barber { get; set; }

        public DbSet<Customer> Customer { get; set; }

        public DbSet<Bookings> Bookings { get; set; }

    }
}
