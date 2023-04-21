namespace Ilyuhin_Backend.Models
{
    public class Bookings
    {
        public int Id { get; set; }

        public int Id_of_customer { get; set; }

        public int Id_of_Barber { get; set; }

        public string Service { get; set; }

        public int Price { get; set; }

        public DateTime Time_of_booking { get; set; }


    }
}
