using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System_wymiany_żywności.Data;
using System_wymiany_żywności.Models;
using System_wymiany_żywności.Services;

namespace System_wymiany_żywności.Controllers
{
    // Wymagamy zalogowania dla całego kontrolera
    [Authorize]
    public class OffersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IExchangeService _exchangeService;

        public OffersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IExchangeService exchangeService)
        {
            _context = context;
            _userManager = userManager;
            _exchangeService = exchangeService;
        }

        // Pobieranie listy ofert - dostępne również dla niezalogowanych
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var offers = await _context.Offers
                .Include(o => o.Category)
                .Include(o => o.User)
                .Where(o => o.Status == OfferStatus.Active)
                .ToListAsync();
            return View(offers);
        }

        // Wyświetlanie szczegółowych informacji o jednej ofercie
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var offer = await _context.Offers
                .Include(o => o.Category)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (offer == null) return NotFound();

            return View(offer);
        }

        // Wyświetlenie formularza dodawania nowej oferty
        public IActionResult Create()
        {
            // Przesłanie listy kategorii do rozwijanego menu w widoku
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // Zapisywanie nowej oferty w bazie danych
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,PriceInPoints,CategoryId")] Offer offer)
        {
            // Powiązanie oferty z aktualnie zalogowanym użytkownikiem
            var user = await _userManager.GetUserAsync(User);
            offer.UserId = user.Id;

            // Nowa oferta trafia do kolejki oczekujących
            offer.Status = OfferStatus.Pending;

            // Ręczne usuwanie błędów walidacji dla obiektów powiązanych
            ModelState.Remove("User");
            ModelState.Remove("Category");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                _context.Add(offer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", offer.CategoryId);
            return View(offer);
        }

        // Obsługa przycisku zakupu/wymiany
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            // Wywołanie zewnętrznego serwisu logiki biznesowej
            var success = await _exchangeService.FinalizeExchangeAsync(id, user.Id);

            if (success)
            {
                TempData["Message"] = "Zakup udany! Punkty pobrane.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = "Błąd zakupu (brak punktów lub oferta nieaktualna).";
                return RedirectToAction(nameof(Details), new { id = id });
            }
        }

        // Przygotowanie danych do edycji własnej oferty
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var offer = await _context.Offers.FindAsync(id);
            if (offer == null) return NotFound();

            // Zabezpieczenie: edytować może tylko właściciel i tylko oferty, które nie są sfinalizowane
            var user = await _userManager.GetUserAsync(User);
            if (offer.UserId != user.Id || offer.Status == OfferStatus.Finalized)
            {
                return Forbid();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", offer.CategoryId);
            return View(offer);
        }

        // Zapisywanie zmian w edytowanej ofercie
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,PriceInPoints,CategoryId")] Offer offer)
        {
            if (id != offer.Id) return NotFound();

            // Pobieramy wersję z bazy bez śledzenia, aby poprawnie zaktualizować tylko wybrane pola
            var existingOffer = await _context.Offers.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
            var user = await _userManager.GetUserAsync(User);

            if (existingOffer == null || existingOffer.UserId != user.Id)
            {
                return Forbid();
            }

            // Zachowujemy oryginalne dane, których nie chcemy zmieniać w edycji
            offer.UserId = existingOffer.UserId;
            offer.Status = existingOffer.Status;
            offer.CreatedAt = existingOffer.CreatedAt;

            ModelState.Remove("User");
            ModelState.Remove("Category");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                _context.Update(offer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", offer.CategoryId);
            return View(offer);
        }

        // Wyświetlenie strony potwierdzenia usunięcia
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var offer = await _context.Offers
                .Include(o => o.Category)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (offer == null) return NotFound();

            // Zabezpieczenie: usuwać może tylko właściciel
            var user = await _userManager.GetUserAsync(User);
            if (offer.UserId != user.Id) return Forbid();

            return View(offer);
        }

        // Faktyczne usunięcie oferty z bazy
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var offer = await _context.Offers.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);

            // Nie pozwalamy usuwać ofert, które zostały już sfinalizowane
            if (offer != null && offer.UserId == user.Id && offer.Status != OfferStatus.Finalized)
            {
                _context.Offers.Remove(offer);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}