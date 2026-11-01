using Sklep_.Models.Sales;

namespace Sklep_.ViewModel;

public class MonthlySummary
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Quantity  { get; set; }
    public decimal Income { get; set; }
    public decimal Profit { get; set; }
    public Profile VintedProfile { get; set; }
}