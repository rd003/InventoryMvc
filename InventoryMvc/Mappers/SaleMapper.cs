using InventoryMvc.Models;
using InventoryMvc.ViewModels;

namespace InventoryMvc.Mappers;

public  static class SaleMapper
{
    public static Sale ToSale(this AddSaleViewModel sale)
    {
        return new Sale
        {
           Id = sale.Id,
           CreateDate = sale.CreateDate,
           Description = sale.Description,
           Price = sale.Price,
           ProductId = sale.ProductId,
           Quantity = sale.Quantity,
           SellingDate = sale.SellingDate ?? DateTime.UtcNow,
        };
    }

    public static AddSaleViewModel ToAddSaleViewModel(this Sale sale)
    {
        return new AddSaleViewModel
        {
            Id = sale.Id,
            CreateDate = sale.CreateDate,
            Description = sale.Description,
            Price = sale.Price,
            ProductId = sale.ProductId,
            Quantity = sale.Quantity,
            SellingDate = sale.SellingDate,
        };
    }
}
