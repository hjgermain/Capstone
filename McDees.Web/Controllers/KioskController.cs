using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using McDees.Web.Models;
using McDees.Web.Services;

namespace McDees.Web.Controllers;

public sealed class KioskController(MenuCatalog catalog, OrderService orders) : Controller
{
    private const string CartKey = "mcdees-cart";
    private const decimal TaxRate = .0785m;

    public IActionResult Index(string? category)
    {
        var categories = catalog.Items.Select(x => x.Category).Distinct().Order().ToList();
        var selected = categories.Contains(category ?? "") ? category! : "All";
        var items = selected == "All" ? catalog.Items : catalog.Items.Where(x => x.Category == selected).ToList();
        return View(new KioskIndexViewModel { Items = items, Categories = categories, SelectedCategory = selected, Cart = GetCart() });
    }

    [HttpGet]
    public IActionResult Customize(int id) => catalog.Find(id) is { } item ? View(item) : NotFound();

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Add(int id, int quantity, int[]? modifiers)
    {
        var item = catalog.Find(id);
        if (item is null) return NotFound();
        var selected = item.Modifiers.Where(x => modifiers?.Contains(x.Id) == true).Select(x => new SelectedModifier(x.Name, x.Price)).ToList();
        var cart = GetCart();
        cart.Add(new CartLine { MenuItemId = item.Id, Name = item.Name, UnitPrice = item.Price, Quantity = Math.Clamp(quantity, 1, 20), Modifiers = selected });
        SaveCart(cart);
        TempData["Message"] = $"{item.Name} added to your order.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult ChangeQuantity(int line, int quantity)
    {
        var cart = GetCart();
        if (line >= 0 && line < cart.Count)
        {
            if (quantity <= 0) cart.RemoveAt(line); else cart[line].Quantity = Math.Clamp(quantity, 1, 20);
            SaveCart(cart);
        }
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Checkout()
    {
        var cart = GetCart();
        return cart.Count == 0 ? RedirectToAction(nameof(Index)) : View(BuildCheckout(cart));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult PlaceOrder()
    {
        var cart = GetCart();
        if (cart.Count == 0) return RedirectToAction(nameof(Index));
        var order = orders.Create(BuildCheckout(cart).Total);
        HttpContext.Session.SetString(CartKey, "[]");
        TempData["OrderNumber"] = order.OrderNumber;
        TempData["PickupCode"] = order.PickupCode;
        TempData["Total"] = order.Total.ToString("0.00");
        return RedirectToAction(nameof(Confirmation));
    }

    public IActionResult Confirmation()
    {
        if (TempData["OrderNumber"] is not string number) return RedirectToAction(nameof(Index));
        return View(new OrderConfirmation(number, (string)TempData["PickupCode"]!, decimal.Parse((string)TempData["Total"]!), DateTimeOffset.Now));
    }

    private List<CartLine> GetCart() => JsonSerializer.Deserialize<List<CartLine>>(HttpContext.Session.GetString(CartKey) ?? "[]") ?? [];
    private void SaveCart(List<CartLine> cart) => HttpContext.Session.SetString(CartKey, JsonSerializer.Serialize(cart));
    private static CheckoutViewModel BuildCheckout(IReadOnlyList<CartLine> cart)
    {
        var subtotal = cart.Sum(x => x.LineTotal);
        var tax = Math.Round(subtotal * TaxRate, 2, MidpointRounding.AwayFromZero);
        return new CheckoutViewModel { Cart = cart, Subtotal = subtotal, Tax = tax, Total = subtotal + tax };
    }
}
