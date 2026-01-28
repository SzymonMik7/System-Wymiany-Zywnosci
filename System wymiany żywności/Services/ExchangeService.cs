using System_wymiany_żywności.Data;
using System_wymiany_żywności.Models;
using Microsoft.EntityFrameworkCore;

namespace System_wymiany_żywności.Services
{
    // Interfejs serwisu do obsługi wymiany
    public interface IExchangeService
    {
        Task<bool> FinalizeExchangeAsync(int offerId, string buyerId);
        Task<bool> ReserveOfferAsync(int offerId, string buyerId);
    }

    // Klasa realizująca główną logikę biznesową projektu
    public class ExchangeService : IExchangeService
    {
        private readonly ApplicationDbContext _context;

        public ExchangeService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Metoda obsługująca proces wymiany
        public async Task<bool> FinalizeExchangeAsync(int offerId, string buyerId)
        {
            // Pobieramy ofertę z bazy (razem z jej właścicielem przez .Include) oraz dane kupującego
            var offer = await _context.Offers.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == offerId);
            var buyer = await _context.Users.FirstOrDefaultAsync(u => u.Id == buyerId);

            // Sprawdzamy warunki czy transakcja jest mozliwa
            if (offer == null || buyer == null || offer.Status != OfferStatus.Active || buyer.WalletBalance < offer.PriceInPoints)
                return false;

            // Logika transakcji
            buyer.WalletBalance -= offer.PriceInPoints;
            offer.User.WalletBalance += offer.PriceInPoints;

            
            buyer.TrustScore += 1;
            offer.User.TrustScore += 1;

            
            offer.Status = OfferStatus.Finalized;

            // Zapisujemy informację o transakcji
            _context.AuditLogs.Add(new AuditLog
            {
                Action = "ExchangeFinalized",
                UserId = buyerId,
                Details = $"Offer {offerId} bought for {offer.PriceInPoints} pts"
            });

            // Zapisujemy wszystkie zmiany w bazie danych
            await _context.SaveChangesAsync();
            return true;
        }
        // Rezerwacja
        public async Task<bool> ReserveOfferAsync(int offerId, string buyerId)
        {
            var offer = await _context.Offers.FirstOrDefaultAsync(o => o.Id == offerId);

            // Walidacja
            if (offer == null || offer.Status != OfferStatus.Active || offer.UserId == buyerId)
            {
                return false;
            }

            // Zmieniamy status oferty
            offer.Status = OfferStatus.Reserved;

            // Tworzymy wpis w tabeli rezerwacji ale ta opcja z rezerwacjami jest do rozwinięcia
            var reservation = new Reservation
            {
                OfferId = offerId,
                BuyerId = buyerId,
                ReservationDate = DateTime.Now
            };
            _context.Reservations.Add(reservation);

            /* Ten fragment zamieniłem na trigger w AppDBCon jako baze na rozrost aplikacji
            _context.AuditLogs.Add(new AuditLog
            {
                Action = "OfferReserved",
                UserId = buyerId,
                Details = $"Offer {offerId} reserved by user {buyerId}",
                Timestamp = DateTime.Now
            });*/

            await _context.SaveChangesAsync();
            return true;
        }
    }
}