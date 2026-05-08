using System.ComponentModel.DataAnnotations;

namespace InventoryMvc.Models;

public class Category
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    public DateTime UpdateDate { get; set; } = DateTime.UtcNow;

    public DateTime? DeleteDate { get; set; }

    [Required]
    [MaxLength(100)]
    public string CategoryName { get; set; } = null!;

    public int? CategoryId { get; set; }

    public Category? CategoryNavigation { get; set; } = null!;

    public List<Category> CategoryInverseNavigation { get; set; } = [];

    public List<Product> Products { get; set; } = [];
}
