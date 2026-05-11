namespace InventoryMvc.ViewModels;

public class ReadPurchaseViewModel
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; }

    public int ProductId { get; set; }

    public int? SupplierId { get; set; }

    public DateTime PurchaseDate { get; set; }

    public decimal Quantity { get; set; }

    public string? Description { get; set; }

    public decimal UnitPrice { get; set; }

    public string? PurchaseOrderNumber { get; set; }

    public string? InvoiceNumber { get; set; }

    public DateTime? ReceivedDate { get; set; }
    public string ProductName { get; set; } = string.Empty;
   // public string? SupplierName { get; set; }
}
