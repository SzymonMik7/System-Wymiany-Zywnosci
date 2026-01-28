using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace System_wymiany_żywności.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int WalletBalance { get; set; } = 100;

        public int TrustScore { get; set; } = 10;


        // Kolekcja ofert, które zostały stworzone przez tego użytkownika (Relacja 1 do wielu) bo jeden użytkownik może mieć wiele ofert
        public ICollection<Offer> Offers { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}