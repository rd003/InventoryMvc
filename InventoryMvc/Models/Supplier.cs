using System.ComponentModel.DataAnnotations;

namespace InventoryMvc.Models;

public class Supplier
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    public DateTime UpdateDate { get; set; } = DateTime.UtcNow;

    public DateTime? DeleteDate { get; set; }

    [Required]
    [MaxLength(100)]
    public string SupplierName { get; set; } = null!;

    public string? ContactPerson { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public string? PostalCode { get; set; }

    public string? TaxNumber { get; set; }

    public int? PaymentTerms { get; set; }

    public bool IsActive { get; set; }

    public List<Product> Products { get; set; } = [];

    public List<Purchase> Purchases { get; set; } = [];
}
