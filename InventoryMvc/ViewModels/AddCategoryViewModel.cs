using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InventoryMvc.ViewModels;

public class AddCategoryViewModel
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; }

    [Required]
    [MaxLength(100)]
    public string CategoryName { get; set; } = null!;

    public int? CategoryId { get; set; }
    public List<SelectListItem> CategoryList { get; set; } = [];
}
