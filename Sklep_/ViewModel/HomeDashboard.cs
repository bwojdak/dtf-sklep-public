
using Sklep_.Models.Sales;
using Sklep_.Models.Stock;

namespace Sklep_.ViewModel;

public class HomeDashboard
{
    public decimal MonthIncome { get; set; }
    public decimal MonthProfit { get; set; }
    public decimal LastMonthProfit { get; set; }
    public int MonthQuantity { get; set; }
    public int ToConfirmQuantity  { get; set; }
    public List<Longsleeve> LowStock { get; set; } = new();
    public List<DesignStats> TopDesigns { get; set; } = new();
    public List<Sale> RecentSales { get; set; } = new();
}