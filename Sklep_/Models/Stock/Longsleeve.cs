using System.ComponentModel.DataAnnotations;

namespace Sklep_.Models.Stock;

public class Longsleeve
{
    public int Id { get; set; }
    public LongsleeveColor Color { get; set; }
    public LongsleeveSize Size { get; set; }
    [Range(0,250, ErrorMessage = "Zakres między 0 a 250")]
    public int Quantity { get; set; }
    [Range(0,250,ErrorMessage = "Zakres między 0 a 250")]
    public decimal UnitCost { get; set; }
}
public enum LongsleeveColor
{
    [Display(Name ="Biały")]
    White,
    [Display(Name ="Czarny")]
    Black
}
public enum LongsleeveSize
{
    M,L,XL
}