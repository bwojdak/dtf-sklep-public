
using Sklep_.Models.Sales;
using Sklep_.Models.Stock;

namespace Sklep_.Models.Vinted;

public class VintedListing
{
    public int Id { get; set; }
    public long VintedItemId { get; set; }
    public decimal Price { get; set; }
    public DateTime LastSeen { get; set; }
    public Profile Profile { get; set; }
    public string ImageUrl { get; set; } = "";
    public string Title { get; set; } = "";
    public ListingState ListingState { get; set; }
    public LongsleeveColor? LongsleeveColor { get; set; }
    public LongsleeveSize? LongsleeveSize { get; set; }
    public int? DesignId { get; set; }
}

public enum ListingState
{
    Posted,
    ToConfirm,
    Sold,
    Rejected
}