using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeShopApp.Models;
using Microsoft.Data.SqlClient;

namespace CoffeeShopApp.Data;

public class SqlOrderItemRepository
{
    private readonly Db _db;
    public SqlOrderItemRepository(Db db) => _db = db;

    public async Task AddAsync(OrderItem i)
    {
    using var c = _db.Open();
    var sql = @"INSERT INTO OrderItems(OrderId,ProductId,Qty,UnitPrice)
            VALUES(@OrderId,@ProductId,@Qty,@UnitPrice)";
    using var cmd = new SqlCommand(sql, c);
        cmd.Parameters.AddWithValue("@OrderId", i.OrderId);
        cmd.Parameters.AddWithValue("@ProductId", i.ProductId);
        cmd.Parameters.AddWithValue("@Qty", i.Qty);
        cmd.Parameters.AddWithValue("@UnitPrice", i.UnitPrice);
        await cmd.ExecuteNonQueryAsync();
    }
}
