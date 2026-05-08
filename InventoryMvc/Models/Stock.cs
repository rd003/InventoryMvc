namespace InventoryMvc.Models;

public class Stock
{
     public int Id { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    public DateTime UpdateDate { get; set; } = DateTime.UtcNow;

    public DateTime? DeleteDate { get; set; }

    public int? ProductId { get; set; }

    public decimal Quantity { get; set; }

    public Product? Product { get; set; }
}
