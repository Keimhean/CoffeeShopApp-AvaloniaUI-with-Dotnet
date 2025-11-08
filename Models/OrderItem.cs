namespace CoffeeShopApp.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal => Qty * UnitPrice;

    // convenience
    public Product? Product { get; set; }
}
