using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sklep_.Data;
using Sklep_.Models.Descriptions;
using Sklep_.Services;

namespace Sklep_.Controllers;

public class DescriptionsController : Controller
{
    private readonly OpenAiService _openAiService;
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public DescriptionsController(OpenAiService openAiService, AppDbContext context,IConfiguration configuration)
    {
        _openAiService = openAiService;
        _context = context;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Index()
    {
        GetDesignsViewBag();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(Description description)
    {
        string opisWzoru = "";
        var wzor = _context.Designs.Find(description.DesignId);
        if (wzor == null)
        {
            ModelState.AddModelError("DesignId", "Nie ma takiego wzoru");
        }
        else
        {
            opisWzoru = wzor.Description;
        }
        var szablon = _configuration["Descriptions:Prompt"];
        if(string.IsNullOrEmpty(szablon))ModelState.AddModelError("", "Brak szablonu opisu w konfiguracji (Descriptions:Prompt)");
        if (string.IsNullOrEmpty(szablon) || !ModelState.IsValid)
        {
            GetDesignsViewBag();
            return View(description);
        }

        var opisy = _context.GeneratedDescriptions.OrderByDescending(g => g.CreatedAt).Take(5).ToList();
        var poprzednieOpisy = string.Join("\n---\n",
            opisy.Select(o => o.Text.Length > 1000 ? o.Text.Substring(0, 1000) : o.Text));
        string wynik;
        try
        {
            wynik = await _openAiService.GenerateAsync(BuildPrompt(szablon,description, opisWzoru, poprzednieOpisy));
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError("", "Nie udało się wygenerować opisu, spróbuj ponownie później" + ex.Message);
            GetDesignsViewBag();
            return View(description);
        }

        var generatedDescription = new GeneratedDescription { CreatedAt = DateTime.Now, Text = wynik };
        _context.GeneratedDescriptions.Add(generatedDescription);
        _context.SaveChanges();
        ViewBag.Wynik = wynik;
        GetDesignsViewBag();
        return View(description);
    }

    private void GetDesignsViewBag()
    {
        ViewBag.Designs = new SelectList(_context.Designs, "Id", "Name");
    }

    private string BuildPrompt(string szablon, Description description, string opisWzoru, string poprzednieOpisy)
    {
        return szablon
            .Replace("{Rozmiar}", description.Size.ToString())
            .Replace("{Kolor}", description.Color.ToString())
            .Replace("{Wzor}", opisWzoru)
            .Replace("{PoprzednieOpisy}", poprzednieOpisy);
    }
}