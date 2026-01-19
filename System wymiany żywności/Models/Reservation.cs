namespace System_wymiany_żywności.Models
{
    // Klasa rezerwacji, aktualnie nie ma takiej opcji w aplikacji, ale wprowadzona w razie rozbudowy 
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime ReservationDate { get; set; } = DateTime.Now;

        public int OfferId { get; set; }
        public Offer Offer { get; set; }

        public string BuyerId { get; set; }
        public ApplicationUser Buyer { get; set; }
    }
}