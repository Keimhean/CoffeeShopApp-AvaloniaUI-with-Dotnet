using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeShopApp.Models;
using Microsoft.Data.SqlClient;

namespace CoffeeShopApp.Data;

public class SqlProductRepository : IRepository<Product>
{
    private readonly Db _db;
    public SqlProductRepository(Db db) => _db = db;

    public async Task<List<Product>> GetAllAsync()
    {
    using var c = _db.Open();
    using var cmd = new SqlCommand("SELECT Id,Name,Category,Price,Stock FROM Products ORDER BY Id", c);
    using var r = await cmd.ExecuteReaderAsync();
        var list = new List<Product>();
        while (await r.ReadAsync())
        {
            list.Add(new Product
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1),
                Category = r.IsDBNull(2) ? null : r.GetString(2),
                Price = r.GetDecimal(3),
                Stock = r.GetInt32(4)
            });
        }
        return list;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
    using var c = _db.Open();
    using var cmd = new SqlCommand("SELECT Id,Name,Category,Price,Stock FROM Products WHERE Id=@Id", c);
        cmd.Parameters.AddWithValue("@Id", id);
        using var r = await cmd.ExecuteReaderAsync();
        if (!await r.ReadAsync()) return null;
        return new Product
        {
            Id = r.GetInt32(0),
            Name = r.GetString(1),
            Category = r.IsDBNull(2) ? null : r.GetString(2),
            Price = r.GetDecimal(3),
            Stock = r.GetInt32(4)
        };
    }

    public async Task<List<Product>> SearchAsync(string q)
    {
    using var c = _db.Open();
    using var cmd = new SqlCommand("SELECT Id,Name,Category,Price,Stock FROM Products WHERE Name LIKE @q ORDER BY Name", c);
        cmd.Parameters.AddWithValue("@q", $"%{q}%");
        using var r = await cmd.ExecuteReaderAsync();
        var list = new List<Product>();
        while (await r.ReadAsync())
        {
            list.Add(new Product
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1),
                Category = r.IsDBNull(2) ? null : r.GetString(2),
                Price = r.GetDecimal(3),
                Stock = r.GetInt32(4)
            });
        }
        return list;
    }

    public async Task<int> AddAsync(Product p)
    {
    using var c = _db.Open();
    var sql = @"INSERT INTO Products(Name,Category,Price,Stock)
            OUTPUT INSERTED.Id VALUES(@Name,@Category,@Price,@Stock)";
    using var cmd = new SqlCommand(sql, c);
        cmd.Parameters.AddWithValue("@Name", p.Name);
    cmd.Parameters.AddWithValue("@Category", (object?)p.Category ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Price", p.Price);
        cmd.Parameters.AddWithValue("@Stock", p.Stock);
    var obj = await cmd.ExecuteScalarAsync();
    return Convert.ToInt32(obj);
    }

    public async Task<bool> UpdateAsync(Product p)
    {
    using var c = _db.Open();
    var sql = @"UPDATE Products SET Name=@Name,Category=@Category,Price=@Price,Stock=@Stock
            WHERE Id=@Id";
    using var cmd = new SqlCommand(sql, c);
        cmd.Parameters.AddWithValue("@Id", p.Id);
        cmd.Parameters.AddWithValue("@Name", p.Name);
    cmd.Parameters.AddWithValue("@Category", (object?)p.Category ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Price", p.Price);
        cmd.Parameters.AddWithValue("@Stock", p.Stock);
        return await cmd.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
    using var c = _db.Open();
    using var cmd = new SqlCommand("DELETE FROM Products WHERE Id=@Id", c);
        cmd.Parameters.AddWithValue("@Id", id);
        return await cmd.ExecuteNonQueryAsync() > 0;
    }
}
