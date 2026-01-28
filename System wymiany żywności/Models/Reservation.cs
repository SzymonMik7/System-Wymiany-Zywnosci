namespace System_wymiany_żywności.Models
{
    // Klasa rezerwacji ale to bardziej na przyszlość do rozwoju bo na razie rezerwacje są prymitywne
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