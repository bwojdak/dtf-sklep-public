using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sklep_.Models.Stock;
using Sklep_.Models.Vinted;

namespace Sklep_.Models.Sales;

public class Sale
{
    public int Id { get; set; }
    [Range(0,1000)]
    public decimal Price { get; set; }
    public DateTime Date { get; set; }
    public Profile VintedProfile { get; set; }
    
    public int DesignId { get; set; }
    public Design? Design { get; set; }
    
    public int LongsleeveId { get; set; }
    public Longsleeve? Longsleeve { get; set; }
    [NotMapped] 
    public string LongsleeveColor { get; set; } = "";
    [NotMapped] 
    public string LongsleeveSize { get; set; } = "";
    public decimal PrintCost { get; set; }
    public decimal LongsleeveCost { get; set; }
    public decimal Profit => Price - LongsleeveCost -  PrintCost;
    public int? VintedListingId { get; set; }
    public VintedListing? VintedListing { get; set; }
}

public enum Profile
{
    Profile2=0,
    Profile1=1
}