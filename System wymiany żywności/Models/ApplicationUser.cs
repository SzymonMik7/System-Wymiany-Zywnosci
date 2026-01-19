using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace System_wymiany_żywności.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int WalletBalance { get; set; } = 100;

        public int TrustScore { get; set; } = 10;


        // Kolekcja ofert, które zostały stworzone przez tego użytkownika (Relacja 1 do wielu)
        public ICollection<Offer> Offers { get; set; }

        // Kolekcja rezerwacji, których dokonał ten użytkownik (Relacja 1 do wielu)
        public ICollection<Reservation> Reservations { get; set; }
    }
}