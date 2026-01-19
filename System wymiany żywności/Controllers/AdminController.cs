using System_wymiany_żywności.Data;
using System_wymiany_żywności.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace System_wymiany_żywności.Controllers
{
    // Ograniczenie dostępu
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context) => _context = context;

        // Akcja wyświetlająca listę ofert, które czekają na akceptację przez admina
        public async Task<IActionResult> PendingOffers()
        {
            var offers = await _context.Offers
                .Include(o => o.User) 
                .Where(o => o.Status == OfferStatus.Pending)
                .ToListAsync();
            return View(offers);
        }

        // Akcja akceptująca ogłoszenie
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var offer = await _context.Offers.FindAsync(id);
            if (offer != null)
            {
                // Zmiana statusu sprawia, że oferta staje się widoczna dla innych użytkowników
                offer.Status = OfferStatus.Active;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(PendingOffers));
        }

        // Akcja generująca raport aktywności użytkowników z bazy danych
        public async Task<IActionResult> Report()
        {
            // Wywołanie procedury składowanej SQL (Stored Procedure) stworzonej wcześniej w bazie
            var reportData = await _context.Database
                .SqlQuery<UserReportViewModel>($"EXEC sp_GetUserReport")
                .ToListAsync();

            return View(reportData);
        }
    }
}