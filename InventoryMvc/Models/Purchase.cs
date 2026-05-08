namespace InventoryMvc.Models;

public class Purchase
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    public DateTime UpdateDate { get; set; } = DateTime.UtcNow;

    public DateTime? DeleteDate { get; set; }

    public int? ProductId { get; set; }

    public int? SupplierId { get; set; }

    public DateTime PurchaseDate { get; set; }

    public decimal Quantity { get; set; }

    public string? Description { get; set; }

    public decimal UnitPrice { get; set; }

    public string? PurchaseOrderNumber { get; set; }

    public string? InvoiceNumber { get; set; }

    public DateTime? ReceivedDate { get; set; }

    public Product? Product { get; set; } = null!;

    public Supplier? Supplier { get; set; } = null!;
}
