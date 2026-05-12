namespace InventoryMvc.ViewModels;

public class SaleDisplayViewModel
{
    public IEnumerable<ReadSaleViewModel> Sales { get; set; } = [];
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalPages { get; set; }
    public string? STerm { get; set; }
}
