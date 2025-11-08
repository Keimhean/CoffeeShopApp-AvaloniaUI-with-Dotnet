using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeShopApp.Models;
using Microsoft.Data.SqlClient;

namespace CoffeeShopApp.Data;

public class SqlOrderRepository
{
    private readonly Db _db;
    public SqlOrderRepository(Db db) => _db = db;

    public async Task<int> CreateAsync(Order o)
    {
    using var c = _db.Open();
    var sql = @"INSERT INTO Orders(OrderNumber,Cashier) OUTPUT INSERTED.Id
            VALUES(@No,@Cashier)";
    using var cmd = new SqlCommand(sql, c);
        cmd.Parameters.AddWithValue("@No", o.OrderNumber);
    cmd.Parameters.AddWithValue("@Cashier", (object?)o.Cashier ?? (object)System.DBNull.Value);

        var obj = await cmd.ExecuteScalarAsync();
    return Convert.ToInt32(obj);
    }

    public async Task<List<Order>> GetRecentAsync(int top = 10)
    {
    using var c = _db.Open();
    // include computed total per order
    using var cmd = new SqlCommand($"SELECT TOP({top}) o.Id,o.OrderNumber,o.Cashier,o.CreatedAt,ISNULL((SELECT SUM(oi.Qty*oi.UnitPrice) FROM OrderItems oi WHERE oi.OrderId=o.Id),0) AS Amount FROM Orders o ORDER BY o.Id DESC", c);
    using var r = await cmd.ExecuteReaderAsync();
        var list = new List<Order>();
        while (await r.ReadAsync())
        {
            list.Add(new Order
            {
                Id = r.GetInt32(0),
                OrderNumber = r.GetString(1),
                Cashier = r.IsDBNull(2) ? null : r.GetString(2),
                CreatedAt = r.GetDateTime(3),
                Amount = r.GetDecimal(4)
            });
        }
        return list;
    }
}
