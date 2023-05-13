namespace Ilyuhin_Backend.Models
{
    public class Barber
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public int Rating { get; set; }

        public int Number_of_reviews { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        //public string Status { get; set; }

    }
}
