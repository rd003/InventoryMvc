using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InventoryMvc.ViewModels;

public class AddPurchaseViewModel
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; } 

    public int ProductId { get; set; }

    public int? SupplierId { get; set; }

    [Required]
    public DateTime? PurchaseDate { get; set; }

    public decimal Quantity { get; set; }

    public string? Description { get; set; }

    public decimal UnitPrice { get; set; }

    public string? PurchaseOrderNumber { get; set; }

    public string? InvoiceNumber { get; set; }

    public DateTime? ReceivedDate { get; set; }
    public List<SelectListItem> ProductList { get; set; } = [];
    public List<SelectListItem> SupplierList { get; set; } = [];
}
