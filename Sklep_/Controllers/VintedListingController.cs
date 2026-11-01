using Microsoft.AspNetCore.Mvc;
using Sklep_.Data;
using Sklep_.Models.Vinted;

namespace Sklep_.Controllers;

public class VintedListingController : Controller
{
    private readonly AppDbContext _context;

    public VintedListingController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var listings = _context.VintedListings
            .Where(l => l.ListingState == ListingState.ToConfirm).ToList();
        return View(listings);
    }

    [HttpPost]
    public IActionResult Reject(int id)
    {
        var listingToReject = _context.VintedListings.FirstOrDefault(l => l.Id == id);
        if (listingToReject != null)
        {
            listingToReject.ListingState = ListingState.Rejected;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        else
        {
            return NotFound();
        }
    }
}