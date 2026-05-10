using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InventoryMvc.ViewModels;

public class AddProductViewModel
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; }

    [Required]
    [MaxLength(100)]
    public string ProductName { get; set; } = null!;

    public int CategoryId { get; set; }

    public decimal Price { get; set; }

    public int? SupplierId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Sku { get; set; } = null!;
    public List<SelectListItem> CategoryList { get; set; } = [];
    public List<SelectListItem> SupplierList { get; set; } = [];
}
