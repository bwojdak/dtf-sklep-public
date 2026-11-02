using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sklep_.Data;
using Sklep_.Models.Sales;
using Sklep_.Models.Vinted;
using Sklep_.ViewModel;

namespace Sklep_.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        var now = DateTime.Now;
        var monthSales = 
            _context.Sales
            .Include(s => s.Design)
            .Where(s => s.Date.Year == now.Year && s.Date.Month == now.Month)
            .ToList();
        var lastMonth = now.AddMonths(-1);
        var homeDashboard = new HomeDashboard
        {
            TopDesigns = monthSales
                .GroupBy(s => s.DesignId)
                .Select(g => new DesignStats
                {
                    Quantity = g.Count(),
                    DesignName = g.First().Design?.Name ?? "",
                })
                .OrderByDescending(d => d.Quantity)
                .Take(5)
                .ToList(),
            LowStock = _context.Longsleeves
                .Where(l => l.Quantity <= 4)
                .OrderBy(l => l.Quantity)
                .ToList(),
            MonthIncome = monthSales.Sum(s => s.Price),
            MonthQuantity = monthSales.Count(),
            ToConfirmQuantity = _context.VintedListings
                .Count(l => l.ListingState == ListingState.ToConfirm),
            MonthProfit = monthSales.Sum(s => s.Profit),
            RecentSales =  _context.Sales
                .Include(s => s.Design)
                .Include(s=>s.Longsleeve)
                .Include(s=>s.VintedListing)
                .OrderByDescending(s => s.Date)
                .Take(5)
                .ToList(),
            LastMonthProfit = _context.Sales
                .Where(s=>s.Date.Month==lastMonth.Month && s.Date.Year==lastMonth.Year)
                .ToList()
                .Sum(s=>s.Profit),
        };
        return View(homeDashboard);
    }
}