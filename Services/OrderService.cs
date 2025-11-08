using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CoffeeShopApp.Data;
using CoffeeShopApp.Models;
using Microsoft.Data.SqlClient;

namespace CoffeeShopApp.Services;

public class OrderService
{
    private readonly Db _db;
    private readonly SqlOrderRepository _orders;
    private readonly SqlOrderItemRepository _items;

    public OrderService(Db db, SqlOrderRepository orders, SqlOrderItemRepository items)
    {
        _db = db; _orders = orders; _items = items;
    }

    public string NewOrderNumber() => $"ORD-{DateTimeOffset.Now.ToUnixTimeSeconds()}";

    public async Task<int> CompleteAsync(Order order)
    {
        // create order
        order.OrderNumber = string.IsNullOrWhiteSpace(order.OrderNumber) ? NewOrderNumber() : order.OrderNumber;
        var id = await _orders.CreateAsync(order);

        // transaction for items + stock deduction
    using var c = _db.Open();
    var tx = (SqlTransaction)await c.BeginTransactionAsync();

        try
        {
            foreach (var it in order.Items)
            {
                // insert item
            var cmd = new SqlCommand(
                    @"INSERT INTO OrderItems(OrderId,ProductId,Qty,UnitPrice) 
                VALUES(@o,@p,@q,@u)", c);
            cmd.Transaction = tx;
                cmd.Parameters.AddWithValue("@o", id);
                cmd.Parameters.AddWithValue("@p", it.ProductId);
                cmd.Parameters.AddWithValue("@q", it.Qty);
                cmd.Parameters.AddWithValue("@u", it.UnitPrice);
                await cmd.ExecuteNonQueryAsync();

                // deduct stock
                var stockCmd = new SqlCommand(
                    @"UPDATE Products SET Stock = Stock - @q WHERE Id=@p", c);
                stockCmd.Transaction = tx;
                stockCmd.Parameters.AddWithValue("@q", it.Qty);
                stockCmd.Parameters.AddWithValue("@p", it.ProductId);
                await stockCmd.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }

        return id;
    }

    public Task<List<Order>> GetRecentAsync(int top = 10) => _orders.GetRecentAsync(top);
}
