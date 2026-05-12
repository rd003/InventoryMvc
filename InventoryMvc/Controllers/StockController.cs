using InventoryMvc.Data;
using InventoryMvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryMvc.Controllers;

public class StockController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<StockController> _logger;
    public StockController(ApplicationDbContext context, ILogger<StockController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string sTerm = "", int page = 1, int limit=3)
    {
        var stockDisplay = new StockDisplayViewModel();
        stockDisplay.STerm = sTerm;
        stockDisplay.Page = page;
        stockDisplay.Limit = limit;
        try
        {
            var stockQuery = _context.Stocks
                .Include(p => p.Product)
                .Select(x => new ReadStockViewModel
                {
                    Id = x.Id,
                    CreateDate = x.CreateDate,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    ProductName = x.Product==null?"": x.Product.ProductName
                });
            if (!string.IsNullOrWhiteSpace(sTerm))
            {
                stockQuery = stockQuery.Where(p => p.ProductName.ToLower().StartsWith(sTerm.ToLower()));
            }
            int totalRecords = stockQuery.Count();
            stockQuery = stockQuery.Skip(limit * (page - 1)).Take(limit);
            int totalPages = (int)Math.Ceiling((double)totalRecords / limit);
            stockDisplay.TotalPages = totalPages;
            stockDisplay.Stocks = await stockQuery.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["Error"] = "Error on fetching stocks";
        }
        return View(stockDisplay);
    }
}
