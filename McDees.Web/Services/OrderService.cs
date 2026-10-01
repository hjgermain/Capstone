using McDees.Web.Models;

namespace McDees.Web.Services;

public sealed class OrderService
{
    private int _nextOrder = 100;
    public OrderConfirmation Create(decimal total) => new(Interlocked.Increment(ref _nextOrder).ToString(), Random.Shared.Next(1000, 10000).ToString(), total, DateTimeOffset.Now);
}
