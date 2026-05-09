namespace InventoryMvc.ViewModels;

public class SupplierDisplayViewModel
{
    public IEnumerable<ReadSupplierViewModel> Suppliers { get; set; } = [];
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalPages { get; set; }
    public string? STerm { get; set; }
}
