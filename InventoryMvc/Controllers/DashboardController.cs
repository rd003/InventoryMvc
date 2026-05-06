using InventoryMvc.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMvc.Controllers;

[Authorize(Roles = Roles.Admin)]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
