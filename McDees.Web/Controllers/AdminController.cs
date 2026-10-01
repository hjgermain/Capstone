using McDees.Web.Data;
using McDees.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace McDees.Web.Controllers;

[Authorize(Roles = "Admin,Manager")]
public sealed class AdminController(ApplicationDbContext db, IdentitySeeder seeder, SignInManager<ApplicationUser> signInManager) : Controller
{
    public IActionResult Index() => View();

    [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
    public async Task<IActionResult> RebuildDatabase(string confirmation)
    {
        if (!string.Equals(confirmation, "REBUILD", StringComparison.Ordinal))
        {
            TempData["Error"] = "Type REBUILD exactly to confirm database destruction.";
            return RedirectToAction(nameof(Index));
        }
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
        await seeder.SeedAsync();
        await signInManager.SignOutAsync();
        TempData["Message"] = "The prototype database was rebuilt. Sign in again to continue.";
        return RedirectToAction("Login", "Account");
    }
}
