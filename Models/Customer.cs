namespace Ilyuhin_Backend.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string Telephone { get; set; }

        public string Email { get; set; }

        public string Comments_on_booking { get; set; }

        public int Number_of_visits { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        //public string Status { get; set; }

    }
}
