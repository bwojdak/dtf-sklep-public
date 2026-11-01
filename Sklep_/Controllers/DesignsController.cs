using Microsoft.AspNetCore.Mvc;
using Sklep_.Data;
using Sklep_.Models;

namespace Sklep_.Controllers;

public class DesignsController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public DesignsController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public IActionResult Index()
    {
        return View(_context.Designs.ToList());
    }

    public IActionResult Details(int id)
    {
        var design = _context.Designs.Find(id);
        if (design == null) return NotFound();
        return View(design);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Design design, IFormFile? image)
    {
        if (!ModelState.IsValid) return View(design);
        if (image != null) design.ImgPath = SaveImage(image);
        _context.Designs.Add(design);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var design = _context.Designs.Find(id);
        if (design == null) return NotFound();
        return View(design);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Design design, IFormFile? image)
    {
        var designtoedit = _context.Designs.Find(design.Id);
        if (designtoedit == null) return NotFound();
        if (!ModelState.IsValid)
        {
            design.ImgPath = designtoedit.ImgPath;
            return View(design);
        }

        designtoedit.PrintCost = design.PrintCost;
        designtoedit.Name = design.Name;
        designtoedit.Type = design.Type;
        designtoedit.Description = design.Description;
        if (image != null)
        {
            DeleteImage(designtoedit.ImgPath);
            designtoedit.ImgPath = SaveImage(image);
        }

        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var design = _context.Designs.Find(id);
        if (design == null) return NotFound();
        var anySales = _context.Sales.Any(s => s.DesignId == design.Id);
        if (anySales)
        {
            TempData["Error"] = "Wzór posiada sprzedaże, nieudane usunięcie";
            return RedirectToAction("Index");
        }

        DeleteImage(design.ImgPath);
        _context.Designs.Remove(design);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    private string SaveImage(IFormFile image)
    {
        var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
        var folder = Path.Combine(_env.WebRootPath, "images");
        Directory.CreateDirectory(folder);
        var filePath = Path.Combine(folder, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        image.CopyTo(stream);
        return "/images/" + fileName;
    }

    private void DeleteImage(string? imgPath)
    {
        if (string.IsNullOrEmpty(imgPath)) return;
        var filePath = Path.Combine(_env.WebRootPath, imgPath.TrimStart('/'));
        if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
    }
}