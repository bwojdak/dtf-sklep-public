using System.ComponentModel.DataAnnotations;
namespace Sklep_.Models;

public class Design
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Podaj nazwę wzoru")]
    public string Name { get; set; } = "";
    [Range(0,100,ErrorMessage = "Podaj wartość pomiędzy 0 a 100")]
    public decimal? PrintCost { get; set; }
    public string Type { get; set; } = "";

    [Required(ErrorMessage = "Wpisz opis wzoru")]
    public string Description { get; set; } = "";
    
    public string? ImgPath  { get; set; }
}