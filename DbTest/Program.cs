using System;
using Npgsql;
using Dapper;

class Program {
    static void Main() {
        var cs = "Host=dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com;Port=5432;Database=billing_system_dtum;Username=admin;Password=3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86";
        using var conn = new NpgsqlConnection(cs);
        conn.Open();
        var cols = conn.Query("SELECT column_name, data_type FROM information_schema.columns WHERE table_name = 'orders'");
        foreach(var c in cols) Console.WriteLine("$"{c.column_name} - {c.data_type}");
    }
}
