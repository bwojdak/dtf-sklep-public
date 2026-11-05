using Microsoft.AspNetCore.Mvc;
using Sklep_.Data;
using Sklep_.Models.Vinted;
using Sklep_.Services;

namespace Sklep_.Controllers;

public class VintedListingController : Controller
{
    private readonly AppDbContext _context;
    private readonly VintedStatus _status;

    public VintedListingController(AppDbContext context,VintedStatus status)
    {
        _context = context;
        _status = status;
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

    public IActionResult LastCheck()
    {
        return Json(new { lastCheck = _status.LastCheck });
    }
}