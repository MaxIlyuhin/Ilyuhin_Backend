namespace Ilyuhin_Backend.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public int Telephone { get; set; }

        public string Email { get; set; }

        public string Comments_on_booking { get; set; }

        public int Number_of_visits { get; set; }

        public enum Status { schoolboy, student }

    }
}
