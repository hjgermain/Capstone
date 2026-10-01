using McDees.Web.Models;
using McDees.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace McDees.Web.Services;

public sealed class OrderService(IHubContext<KitchenHub> kitchenHub)
{
    private int _nextOrder = 100;
    private readonly List<KitchenOrder> _orders = [];
    private readonly object _gate = new();

    public async Task<OrderConfirmation> CreateAsync(IReadOnlyList<CartLine> cart, decimal total)
    {
        var confirmation = new OrderConfirmation(Interlocked.Increment(ref _nextOrder).ToString(), Random.Shared.Next(1000, 10000).ToString(), total, DateTimeOffset.Now);
        lock (_gate)
            _orders.Add(new KitchenOrder { OrderNumber = confirmation.OrderNumber, PickupCode = confirmation.PickupCode, CreatedAt = confirmation.CreatedAt, Lines = cart.Select(x => $"{x.Quantity}× {x.Name}{(x.Modifiers.Count == 0 ? "" : $" — {string.Join(", ", x.Modifiers.Select(m => m.Name))}")}").ToList() });
        await kitchenHub.Clients.All.SendAsync("orderUpdated");
        return confirmation;
    }

    public IReadOnlyList<KitchenOrder> GetKitchenOrders()
    {
        lock (_gate) return _orders.OrderBy(x => x.Status == "Completed").ThenBy(x => x.CreatedAt).ToList();
    }

    public async Task<bool> AdvanceAsync(string orderNumber)
    {
        var states = new[] { "New", "Being Prepared", "Ready", "Completed" };
        var changed = false;
        lock (_gate)
        {
            var order = _orders.SingleOrDefault(x => x.OrderNumber == orderNumber);
            if (order is null) return false;
            var index = Array.IndexOf(states, order.Status);
            if (index < 0 || index == states.Length - 1) return false;
            order.Status = states[index + 1];
            changed = true;
        }
        if (changed) await kitchenHub.Clients.All.SendAsync("orderUpdated");
        return changed;
    }
}
