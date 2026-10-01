using McDees.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace McDees.Web.Controllers;

[Authorize(Roles = "Employee,Manager,Admin")]
public sealed class KitchenController(OrderService orders) : Controller
{
    public IActionResult Index() => View(orders.GetKitchenOrders());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Advance(string orderNumber)
    {
        await orders.AdvanceAsync(orderNumber);
        return RedirectToAction(nameof(Index));
    }
}
