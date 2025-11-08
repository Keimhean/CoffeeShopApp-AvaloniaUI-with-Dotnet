using Microsoft.Data.SqlClient;

namespace CoffeeShopApp.Data;

public sealed class Db
{
    private readonly string _conn;
    public Db(string conn) => _conn = conn;

    public SqlConnection Open()
    {
        var c = new SqlConnection(_conn);
        c.Open();
        return c;
    }
}
