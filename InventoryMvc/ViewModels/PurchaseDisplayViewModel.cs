namespace InventoryMvc.ViewModels;

public class PurchaseDisplayViewModel
{
    public IEnumerable<ReadPurchaseViewModel> Purchases { get; set; } = [];
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalPages { get; set; }
    public string? STerm { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
