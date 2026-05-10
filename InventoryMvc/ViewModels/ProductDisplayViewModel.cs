namespace InventoryMvc.ViewModels;

public class ProductDisplayViewModel
{
    public IEnumerable<ReadProductViewModel> Products { get; set; } = [];
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalPages { get; set; }
    public string? STerm { get; set; }
}
