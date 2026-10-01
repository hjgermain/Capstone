namespace McDees.Web.Models;

public sealed record MenuItem(int Id, string Name, string Category, string Description, decimal Price, string Emoji, string DietaryNote, IReadOnlyList<ModifierOption> Modifiers);
public sealed record ModifierOption(int Id, string Name, decimal Price);
public sealed record SelectedModifier(string Name, decimal Price);

public sealed class CartLine
{
    public int MenuItemId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public int Quantity { get; set; }
    public List<SelectedModifier> Modifiers { get; init; } = [];
    public decimal LineTotal => (UnitPrice + Modifiers.Sum(x => x.Price)) * Quantity;
}

public sealed class KioskIndexViewModel
{
    public IReadOnlyList<MenuItem> Items { get; init; } = [];
    public IReadOnlyList<string> Categories { get; init; } = [];
    public string SelectedCategory { get; init; } = "All";
    public IReadOnlyList<CartLine> Cart { get; init; } = [];
    public decimal CartTotal => Cart.Sum(x => x.LineTotal);
    public int CartCount => Cart.Sum(x => x.Quantity);
}

public sealed class CheckoutViewModel
{
    public IReadOnlyList<CartLine> Cart { get; init; } = [];
    public decimal Subtotal { get; init; }
    public decimal Tax { get; init; }
    public decimal Total { get; init; }
}

public sealed record OrderConfirmation(string OrderNumber, string PickupCode, decimal Total, DateTimeOffset CreatedAt);
