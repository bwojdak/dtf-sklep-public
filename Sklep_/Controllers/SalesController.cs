using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sklep_.Data;
using Sklep_.Models;
using Sklep_.Models.Sales;
using Sklep_.Models.Stock;
using Sklep_.Models.Vinted;
using Sklep_.ViewModel;

namespace Sklep_.Controllers;

public class SalesController : Controller
{
    private readonly AppDbContext _context;

    public SalesController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View(_context.Sales
            .Include(s => s.Design)
            .Include(s => s.Longsleeve)
            .OrderByDescending(s => s.Date)
            .ToList());
    }

    [HttpGet]
    public IActionResult Create(int? vintedListingId)
    {
        var sale = new Sale { Date = DateTime.Now };
        if (vintedListingId != null)
        {
            var listing = _context.VintedListings.FirstOrDefault(l => l.Id == vintedListingId);
            if (listing == null) return NotFound();
            sale.Price = listing.Price;
            sale.Date = listing.LastSeen;
            sale.VintedProfile = listing.Profile;
            sale.LongsleeveSize = listing.LongsleeveSize == null ? "" : ((int)listing.LongsleeveSize).ToString();
            sale.LongsleeveColor = listing.LongsleeveColor == null ? "" : ((int)listing.LongsleeveColor).ToString();
            sale.VintedListingId = listing.Id;
            sale.DesignId = listing.DesignId ?? 0;
        }

        GetDesignsViewBag();
        return View(sale);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Sale sale)
    {
        Longsleeve? matchingLongsleeve = null;
        if (Enum.TryParse<LongsleeveColor>(sale.LongsleeveColor, out var parsedColor) &&
            Enum.TryParse<LongsleeveSize>(sale.LongsleeveSize, out var parsedSize))
        {
            matchingLongsleeve = _context.Longsleeves
                .FirstOrDefault(x => x.Color == parsedColor && x.Size == parsedSize);
            if (matchingLongsleeve == null)
            {
                ModelState.AddModelError("LongsleeveId", "Brak takiego longsleeva w magazynie");
            }
            else if (matchingLongsleeve.Quantity == 0)
            {
                ModelState.AddModelError("LongsleeveId", "Brak dostępnych sztuk");
            }
            else
            {
                sale.LongsleeveId = matchingLongsleeve.Id;
                sale.LongsleeveCost = matchingLongsleeve.UnitCost;
                Design? design = _context.Designs.FirstOrDefault(d => d.Id == sale.DesignId);
                if (design == null) ModelState.AddModelError("DesignId", "Brak takiego designu");
                else
                {
                    sale.PrintCost = design.PrintCost ?? 0;
                }
            }
        }
        else
        {
            ModelState.AddModelError("LongsleeveId", "Nieprawidłowy kolor lub rozmiar");
        }

        if (matchingLongsleeve==null || !ModelState.IsValid)
        {
            GetDesignsViewBag();
            return View(sale);
        }

        if (sale.VintedListingId != null)
        {
            var listing = _context.VintedListings.FirstOrDefault(l => l.Id == sale.VintedListingId);
            if (listing == null) return NotFound();
            listing.ListingState = ListingState.Sold;
        }

        matchingLongsleeve.Quantity--;
        _context.Sales.Add(sale);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var saleToEdit = _context.Sales.Include(s => s.Longsleeve).FirstOrDefault(s => s.Id == id);
        if (saleToEdit?.Longsleeve == null) return NotFound();
        saleToEdit.LongsleeveColor = ((int)saleToEdit.Longsleeve.Color).ToString();
        saleToEdit.LongsleeveSize = ((int)saleToEdit.Longsleeve.Size).ToString();
        GetDesignsViewBag();
        return View(saleToEdit);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Sale sale)
    {
        var saleToEdit = _context.Sales.Include(s => s.Longsleeve).FirstOrDefault(s => s.Id == sale.Id);
        if (saleToEdit == null) return NotFound();
        Longsleeve? matchingLongsleeve = null;
        if (Enum.TryParse<LongsleeveColor>(sale.LongsleeveColor, out var parsedColor) &&
            Enum.TryParse<LongsleeveSize>(sale.LongsleeveSize, out var parsedSize))
        {
            matchingLongsleeve = _context.Longsleeves
                .FirstOrDefault(x => x.Color == parsedColor && x.Size == parsedSize);
            if (matchingLongsleeve == null)
            {
                ModelState.AddModelError("LongsleeveId", "Brak takiego longsleeva w magazynie");
            }
            else if (matchingLongsleeve.Quantity == 0 && saleToEdit.LongsleeveId != matchingLongsleeve.Id)
            {
                ModelState.AddModelError("LongsleeveId", "Brak dostępnych sztuk");
            }
            else
            {
                sale.LongsleeveId = matchingLongsleeve.Id;
            }
        }
        else
        {
            ModelState.AddModelError("LongsleeveId", "Nieprawidłowy kolor lub rozmiar");
        }

        if (saleToEdit.DesignId != sale.DesignId)
        {
            Design? newDesign = _context.Designs.FirstOrDefault(d => d.Id == sale.DesignId);
            if (newDesign == null) ModelState.AddModelError("DesignId", "Brak takiego designu");
            else saleToEdit.PrintCost = newDesign.PrintCost ?? 0;
        }

        if (matchingLongsleeve==null||!ModelState.IsValid)
        {
            GetDesignsViewBag();
            return View(sale);
        }

        if (saleToEdit.LongsleeveId != matchingLongsleeve.Id)
        {
            if (saleToEdit.Longsleeve != null)
            {
                saleToEdit.Longsleeve.Quantity++;
            }

            matchingLongsleeve.Quantity--;
            saleToEdit.LongsleeveId = matchingLongsleeve.Id;
            saleToEdit.LongsleeveCost = matchingLongsleeve.UnitCost;
        }

        saleToEdit.LongsleeveId = matchingLongsleeve.Id;
        saleToEdit.Date = sale.Date;
        saleToEdit.LongsleeveSize = sale.LongsleeveSize;
        saleToEdit.LongsleeveColor = sale.LongsleeveColor;
        saleToEdit.Price = sale.Price;
        saleToEdit.VintedProfile = sale.VintedProfile;
        saleToEdit.DesignId = sale.DesignId;
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id, bool returnToStock)
    {
        Sale? saleToDelete = _context.Sales.Include(s => s.Longsleeve).FirstOrDefault(s => s.Id == id);
        if (saleToDelete == null) return NotFound();
        if (returnToStock && saleToDelete.Longsleeve != null)
        {
            saleToDelete.Longsleeve.Quantity++;
        }

        _context.Sales.Remove(saleToDelete);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    public IActionResult Monthly()
    {
        var sales = _context.Sales.ToList();
        var summaries = sales.GroupBy(s => new { s.Date.Year, s.Date.Month, s.VintedProfile })
            .Select(g => new MonthlySummary
            {
                VintedProfile = g.Key.VintedProfile,
                Year = g.Key.Year,
                Month = g.Key.Month,
                Income = g.Sum(s => s.Price),
                Profit = g.Sum(s => s.Profit),
                Quantity = g.Count()
            })
            .OrderByDescending(m => m.Year)
            .ThenByDescending(m => m.Month)
            .ThenBy(m => m.VintedProfile)
            .ToList();
        return View(summaries);
    }

    public IActionResult DesignStats()
    {
        var designsSold = _context.Sales.Include(s => s.Design).ToList();
        var designsSorted = designsSold.GroupBy(d => new { d.Date.Year, d.Date.Month, d.DesignId })
            .Select(g => new DesignStats
            {
                Quantity = g.Count(),
                DesignName = g.First().Design?.Name ?? " ",
                Year = g.Key.Year,
                Month = g.Key.Month
            })
            .OrderByDescending(m => m.Year)
            .ThenByDescending(m => m.Month)
            .ThenByDescending(m => m.Quantity).ToList();
        return View(designsSorted);
    }

    private void GetDesignsViewBag()
    {
        ViewBag.Designs = new SelectList(_context.Designs, "Id", "Name");
    }
}