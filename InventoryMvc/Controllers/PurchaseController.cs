using InventoryMvc.Constants;
using InventoryMvc.Data;
using InventoryMvc.Mappers;
using InventoryMvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventoryMvc.Controllers;

[Authorize(Roles = Roles.Admin)]
public class CategoryController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ApplicationDbContext context, ILogger<CategoryController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Index(string sTerm = "", int page = 1, int limit = 3)
    {
        var categoryDisplay = new CateogoryDisplayViewModel();
        categoryDisplay.STerm = sTerm;
        categoryDisplay.Page = page;
        categoryDisplay.Limit = limit;
        try
        {
            var categoriesQuery = _context.Categories.Include(c => c.CategoryNavigation)
                .Select(c => new ReadCategoryViewModel
                {
                    CategoryName = c.CategoryName,
                    CategoryId = c.CategoryId,
                    Id = c.Id,
                    ParentCategory = c.CategoryNavigation != null ? c.CategoryNavigation.CategoryName : ""
                });
            if (!string.IsNullOrWhiteSpace(sTerm))
            {
                categoriesQuery = categoriesQuery.Where(c => c.CategoryName.ToLower().StartsWith(sTerm.ToLower()));
            }
            int totalRecords = categoriesQuery.Count();
            categoriesQuery = categoriesQuery.Skip(limit * (page - 1)).Take(limit);
            int totalPages = (int)Math.Ceiling((double)totalRecords / limit);
            categoryDisplay.TotalPages = totalPages;
            categoryDisplay.Categories = categoriesQuery.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["Error"] = "Error on fetching categories";
        }
        return View(categoryDisplay);
    }

    public async Task<IActionResult> AddCategory()
    {
        var categories = await _context.Categories.ToListAsync();
        var categoryViewModel = new AddCategoryViewModel()
        {
            CategoryList = categories.Select(c => new SelectListItem
            {
                Text = c.CategoryName,
                Value = c.Id.ToString()
            }).ToList()
        };
        return View(categoryViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(AddCategoryViewModel category)
    {
        var categories = await _context.Categories.ToListAsync();
        var categoryViewModel = new AddCategoryViewModel()
        {
            CategoryList = categories.Select(c => new SelectListItem
            {
                Text = c.CategoryName,
                Value = c.Id.ToString()
            }).ToList()
        };
        try
        {
            if (!ModelState.IsValid)
            {
                return View(categoryViewModel);
            }
            _context.Categories.Add(category.ToCategory());
            await _context.SaveChangesAsync();
            TempData["Success"] = "Category added";
            return RedirectToAction(nameof(AddCategory));
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error on adding category.";
            _logger.LogError(ex.Message);
            return View(categoryViewModel);
        }
    }

    public async Task<IActionResult> UpdateCategory(int id)
    {
        var categories = await _context.Categories.ToListAsync();
        var category = await _context.Categories.FindAsync(id);
        if (category is null)
        {
            throw new InvalidOperationException("Category does not exists");
        }
        var categoryViewModel = category.ToAddCategoryViewModel();

        categoryViewModel.CategoryList = categories.Select(c => new SelectListItem
        {
            Text = c.CategoryName,
            Value = c.Id.ToString(),
            Selected = c.Id == category.CategoryId
        }).ToList();
        return View(categoryViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCategory(AddCategoryViewModel category)
    {
        var categories = await _context.Categories.AsNoTracking().ToListAsync();
       
        var categoryViewModel = new AddCategoryViewModel()
        {
            CategoryList = categories.Select(c => new SelectListItem
            {
                Text = c.CategoryName,
                Value = c.Id.ToString(),
                Selected = c.Id == category.CategoryId
            }).ToList()
        };
        try
        {
            if (!ModelState.IsValid)
            {
                return View(categoryViewModel);
            }
            var categoryToUpdate = category.ToCategory();
            categoryToUpdate.UpdateDate = DateTime.UtcNow;
            _context.Categories.Update(categoryToUpdate);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Category is updated";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["Error"] = "Category could not be updated";
            return View(categoryViewModel);
        }
    }

    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            var category = await _context.Categories.FindAsync(id);
            if (category is null)
            {
                throw new InvalidOperationException("Category does not exists");
            }
            category.DeleteDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["Error"] = "Category could not be updated";
        }
        return RedirectToAction(nameof(Index));
    }
}
