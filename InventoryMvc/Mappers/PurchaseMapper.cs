using InventoryMvc.Models;
using InventoryMvc.ViewModels;

namespace InventoryMvc.Mappers;

public  static class PurchaseMapper
{
    public static Purchase ToPurchase(this AddPurchaseViewModel purchase)
    {
        return new Purchase
        {
            Id = purchase.Id,
            PurchaseDate = purchase.PurchaseDate,
            PurchaseOrderNumber = purchase.PurchaseOrderNumber,
            CreateDate = purchase.CreateDate,
            InvoiceNumber = purchase.InvoiceNumber,
            UnitPrice = purchase.UnitPrice,
            Description = purchase.Description,
            ProductId = purchase.ProductId,
            SupplierId = purchase.SupplierId,
            Quantity = purchase.Quantity,
            ReceivedDate = purchase.ReceivedDate
        };
    }

    public static AddPurchaseViewModel ToAddPurchaseViewModel(this Purchase purchase)
    {
        return new AddPurchaseViewModel
        {
            Id = purchase.Id,
            PurchaseDate = purchase.PurchaseDate,
            PurchaseOrderNumber = purchase.PurchaseOrderNumber,
            CreateDate = purchase.CreateDate,
            InvoiceNumber = purchase.InvoiceNumber,
            UnitPrice = purchase.UnitPrice,
            Description = purchase.Description,
            ProductId = purchase.ProductId??0,
            SupplierId = purchase.SupplierId,
            Quantity = purchase.Quantity,
            ReceivedDate = purchase.ReceivedDate
        };
    }
}
