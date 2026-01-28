using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace System_wymiany_żywności.Models
{
    public class Offer
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required, Range(0, 1000)]
        public int PriceInPoints { get; set; }

        public OfferStatus Status { get; set; } = OfferStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // FK - Klucz obcy do tabeli użytkowników
        public string UserId { get; set; }
        // Obiekt użytkownika - powiązanie relacyjne jeden do wielu
        public ApplicationUser User { get; set; }

        // FK - Klucz obcy do tabeli kategorii
        public int CategoryId { get; set; }
        // Obiekt kategorii - powiązanie relacyjne jeden do wielu
        public Category Category { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}