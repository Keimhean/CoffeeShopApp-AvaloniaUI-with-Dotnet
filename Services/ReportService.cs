using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using CoffeeShopApp.Data;

namespace CoffeeShopApp.Services;

public class ReportRow { public DateTime Date { get; set; } public int Orders { get; set; } public decimal Revenue { get; set; } }

public class ReportService
{
    private readonly Db _db;
    public ReportService(Db db) => _db = db;

    public async Task<List<ReportRow>> GetDailyAsync(DateTime from, DateTime to)
    {
        using var c = _db.Open();
        using var cmd = new SqlCommand(
            @"SELECT [Date], [Orders], [Revenue]
              FROM v_SalesSummary
              WHERE [Date] BETWEEN @f AND @t
              ORDER BY [Date]", c);
        cmd.Parameters.AddWithValue("@f", from.Date);
        cmd.Parameters.AddWithValue("@t", to.Date);
        using var r = await cmd.ExecuteReaderAsync();
        var list = new List<ReportRow>();
        while (await r.ReadAsync())
        {
            list.Add(new ReportRow {
                Date = r.GetDateTime(0),
                Orders = r.GetInt32(1),
                Revenue = r.GetDecimal(2)
            });
        }
        return list;
    }
}
