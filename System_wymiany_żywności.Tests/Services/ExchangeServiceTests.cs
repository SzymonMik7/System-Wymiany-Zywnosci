using Microsoft.EntityFrameworkCore;
using System_wymiany_żywności.Data;
using System_wymiany_żywności.Models;
using System_wymiany_żywności.Services;
using Xunit;

namespace System_wymiany_żywności.Tests
{
    // Klasa zawierająca testy jednostkowe dla logiki wymiany
    public class ExchangeServiceTests
    {
        // Metoda pomocnicza tworząca "sztuczną" bazę danych w pamięci RAM na potrzeby testów
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        // Test 1: Sprawdzenie, czy punkty są poprawnie przesyłane od kupującego do sprzedawcy
        public async Task FinalizeExchange_Success_TransferPoints()
        {
            var context = GetDbContext();
            var service = new ExchangeService(context);

            var seller = new ApplicationUser { Id = "s1", WalletBalance = 100 };
            var buyer = new ApplicationUser { Id = "b1", WalletBalance = 100 };
            var offer = new Offer { Id = 1, Title = "T", Description = "D", PriceInPoints = 50, UserId = "s1", Status = OfferStatus.Active };

            context.Users.AddRange(seller, buyer);
            context.Offers.Add(offer);
            await context.SaveChangesAsync();

            // Wykonujemy wymianę
            var result = await service.FinalizeExchangeAsync(1, "b1");

            Assert.True(result);
            Assert.Equal(50, buyer.WalletBalance);
            Assert.Equal(150, seller.WalletBalance);
        }

        [Fact]
        // Test 2: Sprawdzenie, czy system zablokuje wymianę, gdy kupujący ma za mało punktów
        public async Task FinalizeExchange_Fail_NotEnoughPoints()
        {
            var context = GetDbContext();
            var service = new ExchangeService(context);

            var buyer = new ApplicationUser { Id = "b2", WalletBalance = 10 }; // Kupujący ma tylko 10 pkt
            var offer = new Offer { Id = 2, Title = "T", Description = "D", PriceInPoints = 50, UserId = "s2", Status = OfferStatus.Active };

            context.Users.Add(buyer);
            context.Offers.Add(offer);
            await context.SaveChangesAsync();

            var result = await service.FinalizeExchangeAsync(2, "b2");

            Assert.False(result);
        }

        [Fact]
        // Test 3: Sprawdzenie, czy system blokuje wymianę ofert, które nie są jeszcze aktywne
        public async Task FinalizeExchange_Fail_OfferNotActive()
        {
            var context = GetDbContext();
            var service = new ExchangeService(context);

            var buyer = new ApplicationUser { Id = "b3", WalletBalance = 100 };
            var offer = new Offer { Id = 3, Title = "T", Description = "D", PriceInPoints = 10, UserId = "s3", Status = OfferStatus.Pending }; // Status oczekujący

            context.Users.Add(buyer);
            context.Offers.Add(offer);
            await context.SaveChangesAsync();

            var result = await service.FinalizeExchangeAsync(3, "b3");

            // Oferta musi być Active, żeby ją kupić
            Assert.False(result);
        }

        [Fact]
        // Test 4: Sprawdzenie, czy po udanej wymianie obu stronom rośnie TrustScore
        public async Task FinalizeExchange_IncreaseTrustScore()
        {
            var context = GetDbContext();
            var service = new ExchangeService(context);

            var seller = new ApplicationUser { Id = "s4", TrustScore = 10 };
            var buyer = new ApplicationUser { Id = "b4", TrustScore = 10 };
            var offer = new Offer { Id = 4, Title = "T", Description = "D", PriceInPoints = 0, UserId = "s4", Status = OfferStatus.Active };

            context.Users.AddRange(seller, buyer);
            context.Offers.Add(offer);
            await context.SaveChangesAsync();

            await service.FinalizeExchangeAsync(4, "b4");

            // Obie strony powinny mieć o 1 punkt reputacji więcej
            Assert.Equal(11, seller.TrustScore);
            Assert.Equal(11, buyer.TrustScore);
        }

        [Fact]
        // Test 5: Sprawdzenie, czy system poprawnie reaguje na próbę zakupu nieistniejącej oferty
        public async Task FinalizeExchange_Fail_InvalidOfferId()
        {
            var context = GetDbContext();
            var service = new ExchangeService(context);

            // Próbujemy kupić ofertę o ID, którego nie ma w bazie
            var result = await service.FinalizeExchangeAsync(999, "any-user");

            Assert.False(result);
        }
    }
}