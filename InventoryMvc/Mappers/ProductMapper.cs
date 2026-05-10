using InventoryMvc.Models;
using InventoryMvc.ViewModels;

namespace InventoryMvc.Mappers;

public  static class ProductMapper
{
    public static Product ToProduct(this AddProductViewModel product)
    {
        return new Product
        {
            Id= product.Id,
            CreateDate = product.CreateDate,
            CategoryId = product.CategoryId,
            Price = product.Price,
            ProductName = product.ProductName,
            Sku = product.Sku,
            SupplierId = product.SupplierId
        };
    }

    public static AddProductViewModel ToAddProductViewModel(this Product product)
    {
        return new AddProductViewModel
        {
            Id = product.Id,
            CreateDate = product.CreateDate,
            CategoryId = product.CategoryId,
            Price = product.Price,
            ProductName = product.ProductName,
            Sku = product.Sku,
            SupplierId = product.SupplierId
        };
    }
}
