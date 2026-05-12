namespace InventoryMvc.ViewModels;

public class ReadStockViewModel
{
    public int Id { get; set; }

    public DateTime CreateDate { get; set; }

    public int? ProductId { get; set; }

    public decimal Quantity { get; set; }

    public string ProductName { get; set; } = string.Empty;
}
