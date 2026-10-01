using McDees.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace McDees.Web.Controllers;

[Authorize(Roles = "Manager,Admin,Owner,Corporate")]
public sealed class InventoryController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index() => View(await db.InventoryItems.OrderBy(x => x.Category).ThenBy(x => x.Name).ToListAsync());
}
