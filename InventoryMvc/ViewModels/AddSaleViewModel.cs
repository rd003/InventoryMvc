using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InventoryMvc.ViewModels;

public class AddSaleViewModel
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; }

    public int ProductId { get; set; }

    [Required]
    public DateTime? SellingDate { get; set; }

    public decimal Quantity { get; set; }

    [Required]
    [MaxLength(300)]
    public string Description { get; set; } = null!;

    public decimal Price { get; set; }
    public List<SelectListItem> ProductList { get; set; } = [];
}
