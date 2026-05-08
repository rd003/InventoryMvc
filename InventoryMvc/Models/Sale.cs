using System.ComponentModel.DataAnnotations;

namespace InventoryMvc.Models;

public class Sale
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    public DateTime UpdateDate { get; set; } = DateTime.UtcNow;

    public DateTime? DeleteDate { get; set; }

    public int ProductId { get; set; }

    public DateTime SellingDate { get; set; }

    public decimal Quantity { get; set; }

    [Required]
    [MaxLength(300)]
    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public Product Product { get; set; } = null!;
}
