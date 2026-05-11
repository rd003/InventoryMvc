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
public class PurchaseController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PurchaseController> _logger;

    public PurchaseController(ApplicationDbContext context, ILogger<PurchaseController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Index(DateTime? startDate, DateTime? endDate, string sTerm = "", int page = 1, int limit = 3)
    {
        var purchaseDisplay = new PurchaseDisplayViewModel();
        purchaseDisplay.STerm = sTerm;
        purchaseDisplay.Page = page;
        purchaseDisplay.Limit = limit;
        try
        {
            var purchaseQuery = _context.Purchases.Include(c => c.Product)
                .Select(p => new ReadPurchaseViewModel
                {
                    Id = p.Id,
                    CreateDate = p.CreateDate,
                    InvoiceNumber = p.InvoiceNumber,
                    ProductId = p.ProductId ?? 0,
                    ProductName = p.Product==null? "": p.Product.ProductName,
                    Description = p.Description,
                    PurchaseDate = p.PurchaseDate,
                    PurchaseOrderNumber = p.PurchaseOrderNumber,
                    Quantity = p.Quantity,
                    ReceivedDate = p.ReceivedDate,
                    SupplierId = p.SupplierId,
                    UnitPrice = p.UnitPrice
                    
                });
            if (!string.IsNullOrWhiteSpace(sTerm))
            {
                purchaseQuery = purchaseQuery.Where(p=> (p.PurchaseOrderNumber??"").ToLower().StartsWith(sTerm.ToLower()));
            }
            if (startDate is not null && endDate is not null) 
            { 
               purchaseQuery = purchaseQuery.Where(p=>p.PurchaseDate >= startDate && p.PurchaseDate <= endDate);
            }
            int totalRecords = purchaseQuery.Count();
            purchaseQuery = purchaseQuery.Skip(limit * (page - 1)).Take(limit);
            int totalPages = (int)Math.Ceiling((double)totalRecords / limit);
            purchaseDisplay.TotalPages = totalPages;
            purchaseDisplay.Purchases = purchaseQuery.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["Error"] = "Error on fetching categories";
        }
        return View(purchaseDisplay);
    }

    public async Task<IActionResult> AddPurchase()
    {
        var products = await _context.Products.ToListAsync();
        var purchaseViewModel = new AddPurchaseViewModel()
        {
            ProductList = products.Select(c => new SelectListItem
            {
                Text = c.ProductName,
                Value = c.Id.ToString()
            }).ToList()
        };
        return View(purchaseViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddPurchase(AddPurchaseViewModel purchase)
    {
        var products = await _context.Products.ToListAsync();
        var purchaseViewModel = new AddPurchaseViewModel()
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
                return View(purchaseViewModel);
            }

            if (purchase.Quantity <= 0)
            {
                throw new InvalidOperationException("Quantity can not be <= 0");
            }

            _context.Purchases.Add(purchase.ToPurchase());

            // purchase 2 iphones, stock->2 (entry in stock)

            var productStock = await _context.Stocks.FirstOrDefaultAsync(s=>s.ProductId==purchase.ProductId);

            if(productStock is null)
            {
                Stock stock = new()
                {
                    ProductId = purchase.ProductId,
                    Quantity = purchase.Quantity
                };
                _context.Add(stock);
            }
            else
            {
                productStock.Quantity += purchase.Quantity;
                productStock.UpdateDate = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Success"] = "Purchase entry is done";
            return RedirectToAction(nameof(AddPurchase));
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error on adding purchase entry.";
            _logger.LogError(ex.Message);
            return View(purchaseViewModel);
        }
    }

    public async Task<IActionResult> UpdatePurchase(int id)
    {
        var products = await _context.Products.ToListAsync();
        var purchase = await _context.Purchases.FindAsync(id);
        if (purchase is null)
        {
            throw new InvalidOperationException("Purchase entry does not exists");
        }
        var purchaseViewModel = purchase.ToAddPurchaseViewModel();

        purchaseViewModel.ProductList = products.Select(c => new SelectListItem
        {
            Text = c.ProductName,
            Value = c.Id.ToString(),
            Selected = c.Id == purchase.ProductId,
        }).ToList();
        return View(purchaseViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePurchase(AddPurchaseViewModel purchase)
    {
        var products = await _context.Products.ToListAsync();

        purchase.ProductList = products.Select(c => new SelectListItem
        {
            Text = c.ProductName,
            Value = c.Id.ToString(),
            Selected = c.Id == purchase.ProductId,
        }).ToList();

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            if (!ModelState.IsValid)
            {
                return View(purchase);
            }

            if (purchase.Quantity <= 0)
            {
                throw new InvalidOperationException("Quantity can not be <= 0");
            }

            var existingPurchase = await _context.Purchases.AsNoTracking().FirstAsync(x=>x.Id==purchase.Id);

            var purchaseToUpdate = purchase.ToPurchase();
            purchaseToUpdate.UpdateDate = DateTime.UtcNow;
            _context.Purchases.Update(purchaseToUpdate);

            // update stock
            // product1-> 2 (qty) stock -> 5-2= 3
            // product1-> 3 (qty) stock -> 3 + (2-3=-1) = 2

            var productStock = await _context.Stocks.FirstOrDefaultAsync(s=>s.ProductId==purchase.ProductId);
            if (productStock is null)
            {
                throw new InvalidOperationException("Product stock is null");
            }
            decimal delta = purchase.Quantity - existingPurchase.Quantity;
            // delta = new - old
            // old qty: 2, new quantity:3 , delta = 3-2 = 1 (stock increases)
            // old : 3, new 2, delta = 2-3 = -1 (stock decreases)
            productStock.Quantity += delta;
            productStock.UpdateDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            TempData["Success"] = "Purchase entry is updated";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["Error"] = "Purchase entry be updated";
            return View(purchase);
        }
    }

    public async Task<IActionResult> DeletePurchase(int id)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase is null)
            {
                throw new InvalidOperationException("Purchase does not exists");
            }
            purchase.DeleteDate = DateTime.UtcNow;

            var productStock = await _context.Stocks.FirstOrDefaultAsync(x => x.ProductId == purchase.ProductId);
            if (productStock is null)
            {
                throw new InvalidOperationException("Can not update the product stock, because product stock does not exist.");
            }
            productStock.Quantity += purchase.Quantity;
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["Error"] = "Category could not be updated";
        }
        return RedirectToAction(nameof(Index));
    }
}
