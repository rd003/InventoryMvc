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
public class SupplierController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SupplierController> _logger;

    public SupplierController(ApplicationDbContext context, ILogger<SupplierController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Index(string sTerm = "", int page = 1, int limit = 3)
    {
        var supplierDisplayViewModel = new SupplierDisplayViewModel();
        supplierDisplayViewModel.STerm = sTerm;
        supplierDisplayViewModel.Page = page;
        supplierDisplayViewModel.Limit = limit;
        try
        {
            var suppliersQuery = _context.Suppliers.AsQueryable();
            if (!string.IsNullOrWhiteSpace(sTerm))
            {
                suppliersQuery = suppliersQuery.Where(c => c.SupplierName.ToLower().StartsWith(sTerm.ToLower()));
            }
            int totalRecords = suppliersQuery.Count();
            suppliersQuery = suppliersQuery.Skip(limit * (page - 1)).Take(limit);
            int totalPages = (int)Math.Ceiling((double)totalRecords / limit);
            supplierDisplayViewModel.TotalPages = totalPages;
            supplierDisplayViewModel.Suppliers = suppliersQuery.Select(s=>s.ToReadSupplierViewModel()).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["Error"] = "Error on fetching suppliers";
        }
        return View(supplierDisplayViewModel);
    }

    public async Task<IActionResult> AddSupplier()
    {
       return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddSupplier(AddSupplierViewModel supplier)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            _context.Suppliers.Add(supplier.ToSupplier());
            await _context.SaveChangesAsync();
            TempData["Success"] = "Supplier added";
            return RedirectToAction(nameof(AddSupplier));
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error on adding Supplier.";
            _logger.LogError(ex.Message);
            return View();
        }
    }

    public async Task<IActionResult> UpdateSupplier(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier is null)
        {
            throw new InvalidOperationException("Supplier does not exists");
        }
        var supplierViewModel = supplier.ToSupplierViewModel();
        return View(supplierViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSupplier(AddSupplierViewModel supplier)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var supplierToUpdate = supplier.ToSupplier();
            supplierToUpdate.UpdateDate = DateTime.UtcNow;
            _context.Suppliers.Update(supplierToUpdate);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Supplier is updated";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["Error"] = "Supplier could not be updated";
            return View();
        }
    }

    public async Task<IActionResult> DeleteSupplier(int id)
    {
        try
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier is null)
            {
                throw new InvalidOperationException("Supplier does not exists");
            }
            supplier.DeleteDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            TempData["Error"] = "Supplier could not be updated";
        }
        return RedirectToAction(nameof(Index));
    }
}
