using Microsoft.AspNetCore.Mvc;
using Sklep_.Data;
using Sklep_.Models.Stock;

namespace Sklep_.Controllers;

public class LongsleevesController : Controller
{
    private readonly AppDbContext _context;

    public LongsleevesController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View(_context.Longsleeves.OrderBy(s=>s.Color).ThenBy(s=>s.Size).ToList());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Longsleeve longsleeve)
    {
        if (!ModelState.IsValid) return View(longsleeve);
        var existing =
            _context.Longsleeves.FirstOrDefault(x => x.Color == longsleeve.Color && x.Size == longsleeve.Size);
        if (existing != null)
        {
            if (existing.Quantity + longsleeve.Quantity > 250)
            {
                int quantityleft = 250 - existing.Quantity;
                ModelState.AddModelError("Quantity",
                    $"Ilość nie może być większa od 250 możesz dodać jeszcze {quantityleft} sztuk");
                return View(longsleeve);
            }

            if (existing.Quantity + longsleeve.Quantity > 0)
            {
                decimal newCost = Math.Round(
                    (existing.UnitCost * existing.Quantity
                     + longsleeve.UnitCost * longsleeve.Quantity)
                    / (existing.Quantity + longsleeve.Quantity), 2);
                existing.UnitCost = newCost;
            }

            existing.Quantity += longsleeve.Quantity;
        }
        else
        {
            _context.Longsleeves.Add(longsleeve);
        }

        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var longsleeve = _context.Longsleeves.Find(id);
        if (longsleeve == null) return NotFound();
        return View(longsleeve);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Longsleeve longsleeve)
    {
        var longsleevetoedit = _context.Longsleeves.Find(longsleeve.Id);
        if (longsleevetoedit == null) return NotFound();
        if (!ModelState.IsValid) return View(longsleevetoedit);
        longsleevetoedit.Quantity = longsleeve.Quantity;
        longsleevetoedit.UnitCost = longsleeve.UnitCost;
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var longsleevetodelete = _context.Longsleeves.Find(id);
        if (longsleevetodelete == null) return NotFound();
        var hasAnySales = _context.Sales.Any(s => s.LongsleeveId == longsleevetodelete.Id);
        if (hasAnySales)
        {
            TempData["Error"] = "Nie można usunąć, posiada sprzedaże";
            return RedirectToAction("Index");
        }

        _context.Longsleeves.Remove(longsleevetodelete);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
}