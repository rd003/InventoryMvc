using System.ComponentModel.DataAnnotations;

namespace InventoryMvc.ViewModels;

public class ReadCategoryViewModel
{
    public int Id { get; set; }

    public string CategoryName { get; set; } = null!;

    public int? CategoryId { get; set; }

    public string ParentCategory { get; set; }
}
