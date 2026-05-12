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
public class SaleController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SaleController> _logger;

    public SaleController(ApplicationDbContext context, ILogger<SaleController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Index(DateTime? startDate, DateTime? endDate, int page = 1, int limit = 3)
    {
        var saleDisplay = new SaleDisplayViewModel();
        //saleDisplay.STerm = sTerm;
        saleDisplay.Page = page;
        saleDisplay.Limit = limit;
        saleDisplay.StartDate = startDate;
        saleDisplay.EndDate = endDate;
        try
        {
            var saleQuery = _context.Sales.Include(c => c.Product)
                .Select(s => new ReadSaleViewModel
                {
                    Id = s.Id,
                    CreateDate = s.CreateDate,
                    ProductId = s.ProductId,
                    ProductName = s.Product.ProductName,
                    Description = s.Description ?? "",
                    Price = s.Price,
                    Quantity = s.Quantity,
                    SellingDate = s.SellingDate
                });
            //if (!string.IsNullOrWhiteSpace(sTerm))
            //{
            //    saleQuery = saleQuery.Where(p=> (p.PurchaseOrderNumber??"").ToLower().StartsWith(sTerm.ToLower()));
            //}
            if (startDate is not null && endDate is not null)
            {
                saleQuery = saleQuery.Where(s => s.SellingDate >= startDate && s.SellingDate <= endDate);
            }
            int totalRecords = saleQuery.Count();
            saleQuery = saleQuery.Skip(limit * (page - 1)).Take(limit);
            int totalPages = (int)Math.Ceiling((double)totalRecords / limit);
            saleDisplay.TotalPages = totalPages;
            saleDisplay.Sales = saleQuery.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["Error"] = "Error on fetching categories";
        }
        return View(saleDisplay);
    }

    public async Task<IActionResult> AddSale()
    {
        var products = await _context.Products.ToListAsync();
        var saleViewModel = new AddSaleViewModel()
        {
            ProductList = products.Select(c => new SelectListItem
            {
                Text = c.ProductName,
                Value = c.Id.ToString()
            }).ToList()
        };
        return View(saleViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddSale(AddSaleViewModel sale)
    {
        var products = await _context.Products.ToListAsync();
        var saleViewModel = new AddSaleViewModel()
        {
            ProductList = products.Select(c => new SelectListItem
            {
                Text = c.ProductName,
                Value = c.Id.ToString()
            }).ToList()
        };
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (!ModelState.IsValid)
            {
                return View(saleViewModel);
            }

            if (sale.Quantity <= 0)
            {
                throw new InvalidOperationException("Quantity can not be <= 0");
            }

            _context.Sales.Add(sale.ToSale());

            // purchase 2 iphones, stock->2 (entry in stock)

            var productStock = await _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == sale.ProductId);

            if (productStock is null)
            {
                throw new InvalidOperationException("Product stock is null");
            }

            if (sale.Quantity > productStock.Quantity)
            {
                throw new InvalidOperationException("Sale quantitiy can not exceed product stock quantity");
            }

            productStock.Quantity -= sale.Quantity;
            productStock.UpdateDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Success"] = "Sale entry is done";
            return RedirectToAction(nameof(AddSale));
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error on adding sale entry.";
            _logger.LogError(ex.Message);
            return View(saleViewModel);
        }
    }

    public async Task<IActionResult> UpdateSale(int id)
    {
        var products = await _context.Products.ToListAsync();
        var sale = await _context.Sales.FindAsync(id);
        if (sale is null)
        {
            throw new InvalidOperationException("Sale entry does not exists");
        }
        var saleViewModel = sale.ToAddSaleViewModel();

        saleViewModel.ProductList = products.Select(c => new SelectListItem
        {
            Text = c.ProductName,
            Value = c.Id.ToString(),
            Selected = c.Id == sale.ProductId,
        }).ToList();
        return View(saleViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSale(AddSaleViewModel sale)
    {
        var products = await _context.Products.ToListAsync();

        sale.ProductList = products.Select(c => new SelectListItem
        {
            Text = c.ProductName,
            Value = c.Id.ToString(),
            Selected = c.Id == sale.ProductId,
        }).ToList();

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            if (!ModelState.IsValid)
            {
                return View(sale);
            }

            if (sale.Quantity <= 0)
            {
                throw new InvalidOperationException("Quantity can not be <= 0");
            }

            var existingSale = await _context.Sales.AsNoTracking().FirstAsync(x => x.Id == sale.Id);

            if(existingSale is null)
            {
                throw new InvalidOperationException("Sale for this id does not exists.");
            }

            var saleToUpdate = sale.ToSale();
            saleToUpdate.UpdateDate = DateTime.UtcNow;
            _context.Sales.Update(saleToUpdate);

            // update stock

            var productStock = await _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == sale.ProductId);
            if (productStock is null)
            {
                throw new InvalidOperationException("Product stock is null");
            }
            decimal delta = sale.Quantity - existingSale.Quantity;
            // delta = new - old
            // old qty: 2, new quantity:3 , delta = 3-2 = 1 (stock decreases)
            // old : 3, new 2, delta = 2-3 = -1 (stock increases)

            if(delta>0 && delta > productStock.Quantity)
            {
                throw new InvalidOperationException("You can not sell more than stock.");
            }

            productStock.Quantity -= delta;
            productStock.UpdateDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            TempData["Success"] = "Sale entry is updated";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["Error"] = "Sale entry can not be updated";
            return View(sale);
        }
    }

    public async Task<IActionResult> DeleteSale(int id)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale is null)
            {
                throw new InvalidOperationException("Sale does not exists");
            }
            sale.DeleteDate = DateTime.UtcNow;

            var productStock = await _context.Stocks.FirstOrDefaultAsync(x => x.ProductId == sale.ProductId);
            if (productStock is null)
            {
                throw new InvalidOperationException("Can not update the product stock, because product stock does not exist.");
            }
            productStock.Quantity += sale.Quantity;
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["Error"] = "Sale entry could not be deleted";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetProductPrice(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product is null) return NotFound();
        return Json(new { price = product.Price });
    }
}
