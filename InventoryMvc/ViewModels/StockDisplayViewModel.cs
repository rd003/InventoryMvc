namespace InventoryMvc.ViewModels;

public class StockDisplayViewModel
{
    public IEnumerable<ReadStockViewModel> Stocks { get; set; } = [];
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalPages { get; set; }
    public string? STerm { get; set; }
}
