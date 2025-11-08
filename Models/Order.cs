using System;
using System.Collections.Generic;
using System.Linq;
using CoffeeShopApp.Utils;

namespace CoffeeShopApp.Models;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = "";
    public string? Cashier { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public List<OrderItem> Items { get; set; } = new();

    // computed from items in-memory
    public decimal Total => Items.Sum(i => i.Subtotal);

    // persisted/queried total (for recent list queries)
    public decimal Amount { get; set; }

    // Convert CreatedAt to Cambodia local time for display
    public DateTime CreatedAtLocal => TimeZoneHelper.ToCambodiaTime(CreatedAt);

    // Human-friendly created-at text in Cambodia time
    public string CreatedAtLocalString => CreatedAtLocal.ToString("g");
}
