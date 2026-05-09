using InventoryMvc.Models;
using InventoryMvc.ViewModels;

namespace InventoryMvc.Mappers;

public  static class CategoryMapper
{
    public static Category ToCategory(this AddCategoryViewModel category)
    {
        return new Category
        {
            Id=category.Id,
            CategoryName =category.CategoryName,
            CategoryId = category.CategoryId,
            CreateDate = category.CreateDate
        };
    }

    public static AddCategoryViewModel ToAddCategoryViewModel(this Category category)
    {
        return new AddCategoryViewModel
        {
            Id = category.Id,
            CategoryName = category.CategoryName,
            CategoryId = category.CategoryId,
            CreateDate = category.CreateDate
        };
    }
}
