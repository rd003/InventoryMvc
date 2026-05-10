using InventoryMvc.Constants;
using InventoryMvc.Data;
using InventoryMvc.Mappers;
using InventoryMvc.Models;
using InventoryMvc.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventoryMvc.Controllers;

[Authorize(Roles = Roles.Admin)]
public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductController> _logger;

    public ProductController(ApplicationDbContext context, ILogger<ProductController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string sTerm = "", int page = 1, int limit = 3)
    {
        var productDisplay = new ProductDisplayViewModel();
        productDisplay.STerm = sTerm;
        productDisplay.Page = page;
        productDisplay.Limit = limit;
        try
        {
            var productsQuery = _context.Products
                .Include(p=>p.Supplier)
                .Include(p=>p.Category)
                .Select(x => new ReadProductViewModel
                {
                   Id = x.Id,
                   CategoryId = x.CategoryId,
                   CategoryName = x.Category.CategoryName,
                   Price = x.Price,
                   ProductName = x.ProductName,
                   CreateDate = x.CreateDate,
                   Sku = x.Sku,
                   SupplierId = x.SupplierId,
                   SupplierName = x.Supplier == null? "": x.Supplier.SupplierName
                });
            if (!string.IsNullOrWhiteSpace(sTerm))
            {
                productsQuery = productsQuery.Where(p => p.ProductName.ToLower().StartsWith(sTerm.ToLower()));
            }
            int totalRecords = productsQuery.Count();
            productsQuery = productsQuery.Skip(limit * (page - 1)).Take(limit);
            int totalPages = (int)Math.Ceiling((double)totalRecords / limit);
            productDisplay.TotalPages = totalPages;
            productDisplay.Products = await productsQuery.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["Error"] = "Error on fetching categories";
        }
        return View(productDisplay);
    }

    public async Task<IActionResult> AddProduct()
    {
        var categories = await _context.Categories.ToListAsync();
        var suppliers = await _context.Suppliers.ToListAsync();
        var productViewModel = new AddProductViewModel()
        {
            CategoryList = categories.Select(c => new SelectListItem
            {
                Text = c.CategoryName,
                Value = c.Id.ToString()
            }).ToList(),
            SupplierList = suppliers.Select(s=> new SelectListItem
            { 
              Text = s.SupplierName,
              Value = s.Id.ToString()
            }).ToList()
        };
        return View(productViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct(AddProductViewModel product)
    {
        var categories = await _context.Categories.ToListAsync();
        var suppliers = await _context.Suppliers.ToListAsync();
        var productViewModel = new AddProductViewModel()
        {
            CategoryList = categories.Select(c => new SelectListItem
            {
                Text = c.CategoryName,
                Value = c.Id.ToString()
            }).ToList(),
            SupplierList = suppliers.Select(s => new SelectListItem
            {
                Text = s.SupplierName,
                Value = s.Id.ToString()
            }).ToList()
        };
        try
        {
            if (!ModelState.IsValid)
            {
                return View(productViewModel);
            }
            _context.Products.Add(product.ToProduct());
            await _context.SaveChangesAsync();
            TempData["Success"] = "Product added";
            return RedirectToAction(nameof(AddProduct));
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error on adding product.";
            _logger.LogError(ex.Message);
            return View(productViewModel);
        }
    }

    public async Task<IActionResult> UpdateProduct(int id)
    {
        var categories = await _context.Categories.ToListAsync();
        var suppliers = await _context.Suppliers.ToListAsync();

        var product = await _context.Products.FindAsync(id);
        if (product is null)
        {
            throw new InvalidOperationException("Product does not exists");
        }
        var productViewModel = product.ToAddProductViewModel();

        productViewModel.CategoryList = categories.Select(c => new SelectListItem
        {
            Text = c.CategoryName,
            Value = c.Id.ToString(),
            Selected = c.Id == product.CategoryId
        }).ToList();

        productViewModel.SupplierList = suppliers.Select(c => new SelectListItem
        {
            Text = c.SupplierName,
            Value = c.Id.ToString(),
            Selected = c.Id == product.SupplierId
        }).ToList();
        return View(productViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProduct(AddProductViewModel product)
    {
        var categories = await _context.Categories.AsNoTracking().ToListAsync();
        var suppliers = await _context.Suppliers.ToListAsync();

        var productViewModel = new AddProductViewModel()
        {
            CategoryList = categories.Select(c => new SelectListItem
            {
                Text = c.CategoryName,
                Value = c.Id.ToString(),
                Selected = c.Id == product.CategoryId
            }).ToList()
        };
        productViewModel.SupplierList = suppliers.Select(c => new SelectListItem
        {
            Text = c.SupplierName,
            Value = c.Id.ToString(),
            Selected = c.Id == product.SupplierId
        }).ToList();
        try
        {
            if (!ModelState.IsValid)
            {
                return View(productViewModel);
            }
            var productToUpdate = product.ToProduct();
            productToUpdate.UpdateDate = DateTime.UtcNow;
            _context.Products.Update(productToUpdate);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Product is updated";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["Error"] = "Product could not be updated";
            return View(productViewModel);
        }
    }

    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
            {
                throw new InvalidOperationException("Product does not exists");
            }
            product.DeleteDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["Error"] = "Product could not be deleted";
        }
        return RedirectToAction(nameof(Index));
    }
}
