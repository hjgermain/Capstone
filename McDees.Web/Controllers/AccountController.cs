using McDees.Web.Data;
using McDees.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace McDees.Web.Controllers;

public sealed class AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager) : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl) => View(new LoginViewModel { ReturnUrl = returnUrl });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "We couldn’t find an account with that email and password.");
            return View(model);
        }
        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)) return LocalRedirect(model.ReturnUrl);
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user is not null && (await userManager.IsInRoleAsync(user, "Admin") || await userManager.IsInRoleAsync(user, "Manager"))) return RedirectToAction("Index", "Admin");
        if (user is not null && await userManager.IsInRoleAsync(user, "Employee")) return RedirectToAction("Index", "Staff");
        return RedirectToAction("Index", "Kiosk");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Kiosk");
    }
}
