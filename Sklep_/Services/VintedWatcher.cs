using System.Globalization;
using AngleSharp.Html.Parser;
using Sklep_.Data;
using Sklep_.Models;
using Sklep_.Models.Sales;
using Sklep_.Models.Stock;
using Sklep_.Models.Vinted;


namespace Sklep_.Services;

public class VintedWatcher : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public VintedWatcher(IServiceScopeFactory scopeFactory, IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckProfileAsync(_configuration["Vinted:Profile1Url"],
                    Profile.Profile1, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Profile1 Error {ex.Message}");
            }

            try
            {
                await CheckProfileAsync(_configuration["Vinted:Profile2Url"],
                    Profile.Profile2, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Profile2 Error: {ex.Message}");
            }


            await Task.Delay(900000, stoppingToken);
        }
    }

    private async Task CheckProfileAsync(string? url, Profile profile, CancellationToken stoppingToken)
    {
        if(string.IsNullOrEmpty(url))
        {
            Console.WriteLine($"{profile}: brak adresu profilu w konfiguracji, pomijam");
            return;
        }
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(url, stoppingToken);
        var html = await response.Content.ReadAsStringAsync(stoppingToken);

        //parsowanie htmla na drzewko i utworzenie linkow, kazdy zawiera a[data-testid$='--overlay-link']
        var parser = new HtmlParser();
        var document = parser.ParseDocument(html);
        var links = document.QuerySelectorAll("a[data-testid$='--overlay-link']");
        if (!response.IsSuccessStatusCode || links.Length == 0)
        {
            return;
        }

        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var openAi = scope.ServiceProvider.GetRequiredService<OpenAiService>();
            var seenIds = new List<long>();
            var designsList = context.Designs.ToList();
            foreach (var link in links)
            {
                //wyciagniecie itemid
                var id = link.GetAttribute("href");
                if (id == null) continue;
                id = id.Replace("/items/", "");
                long vintedItemId = long.Parse(id);
                seenIds.Add(vintedItemId);

                //wyciagniecie samego tytulu ogloszenia
                var title = link.GetAttribute("title");
                if (title == null) continue;
                int markaIndex = title.IndexOf(", marka:", StringComparison.Ordinal);
                string shortTitle = markaIndex >= 0 ? title.Substring(0, markaIndex) : title;

                var parts = title.Split(", ");

                //wyciagniecie rozmiaru i parse na enuma
                var sizePart = parts.FirstOrDefault(s => s.StartsWith("rozmiar:"));
                sizePart = sizePart?.Replace("rozmiar: ", "");
                LongsleeveSize? size = null;
                if (Enum.TryParse(sizePart, out LongsleeveSize sizeEnum))
                {
                    size = sizeEnum;
                }

                //wyciagniecie ceny
                var pricePart = parts.FirstOrDefault(s => s.EndsWith("zł"));
                if (pricePart == null) continue;
                pricePart = pricePart.Replace(" zł", "");
                decimal price = decimal.Parse(pricePart, new CultureInfo("pl-PL"));

                //imageurl
                var img = document.QuerySelector($"img[data-testid='product-item-id-{vintedItemId}--image--img']");
                var imageUrl = img?.GetAttribute("src");


                var existingListing = context.VintedListings.FirstOrDefault(l => l.VintedItemId == vintedItemId);
                if (existingListing != null)
                {
                    if (existingListing.ListingState == ListingState.ToConfirm)
                        existingListing.ListingState = ListingState.Posted;
                    existingListing.LastSeen = DateTime.Now;
                    existingListing.Price = price;
                }
                else
                {
                    var color = await RecognizeColorAsync(openAi, imageUrl);
                    var designId = await RecognizeDesignAsync(openAi, imageUrl, shortTitle, designsList);
                    var newListing = new VintedListing
                    {
                        LongsleeveColor = color,
                        DesignId = designId,
                        VintedItemId = vintedItemId,
                        Price = price,
                        LastSeen = DateTime.Now,
                        ImageUrl = imageUrl ?? "",
                        ListingState = ListingState.Posted,
                        Profile = profile,
                        Title = shortTitle,
                        LongsleeveSize = size
                    };
                    context.VintedListings.Add(newListing);
                }
            }

            var missing = context.VintedListings
                .Where(l => l.Profile == profile && l.ListingState == ListingState.Posted &&
                            !seenIds.Contains(l.VintedItemId)).ToList();
            foreach (var listing in missing)
            {
                listing.ListingState = ListingState.ToConfirm;
            }

            context.SaveChanges();
        }
    }

    private async Task<LongsleeveColor?> RecognizeColorAsync(OpenAiService openAiService, string? imageUrl)
    {
        if (imageUrl == null) return null;
        try
        {
            var prompt = "Jaki kolor ma longsleeve na zdjęciu, odpowiedz jednym słowem: White albo Black";
            var result = await openAiService.GenerateFromImageAsync(prompt, imageUrl);
            if (Enum.TryParse(result.Trim(), true, out LongsleeveColor color))
            {
                return color;
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd rozpoznawania koloru: {ex.Message}");
            return null;
        }
    }

    private async Task<int?> RecognizeDesignAsync(OpenAiService openAiService, string? imageUrl, string title,
        List<Design>? designs)
    {
        if (imageUrl == null || designs == null) return null;
        try
        {
            var listaWzorow = string.Join("\n", designs.Select(d => $"{d.Id}: {d.Name} - {d.Description}"));
            var prompt = $"""
                          Na zdjęciu jest longsleeve z nadrukiem. Tytuł oferty: {title}
                          Lista wzorów:
                          {listaWzorow}
                          Który wzór jest na zdjęciu? Odpowiedz samym numerem. Jeśli żaden nie pasuje, odpowiedz 0.
                          """;
            var result = await openAiService.GenerateFromImageAsync(prompt, imageUrl);
            if (int.TryParse(result.Trim(), out int designId) && designs.Any(d => d.Id == designId))
            {
                return designId;
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd rozpoznawania designu: {ex.Message}");
            return null;
        }
    }
}