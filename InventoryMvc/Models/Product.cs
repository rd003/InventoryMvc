using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace InventoryMvc.Models;

public class Product
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    public DateTime UpdateDate { get; set; } = DateTime.UtcNow;

    public DateTime? DeleteDate { get; set; }

    [Required]
    [MaxLength(100)]
    public string ProductName { get; set; } = null!;

    public int CategoryId { get; set; }

    public decimal Price { get; set; }

    public int? SupplierId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Sku { get; set; } = null!;

    public Category Category { get; set; } = null!;

    public List<Purchase> Purchases { get; set; } = [];

    public List<Sale> Sales { get; set; } = [];

    public Stock? Stock { get; set; }

    public Supplier? Supplier { get; set; }
}
