using System.ComponentModel.DataAnnotations;

namespace InventoryMvc.ViewModels;

public class ReadSaleViewModel
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; } 

    public int ProductId { get; set; }

    public DateTime SellingDate { get; set; }

    public decimal Quantity { get; set; }

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }
    public string ProductName { get; set; }
}
