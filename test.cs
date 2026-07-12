using System;
using Npgsql;
class Program {
    static void Main() {
        var conn = new NpgsqlConnection("Host=dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com;Port=5432;Database=billing_system_dtum;Username=admin;Password=3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86");
        conn.Open();
        var cmd = new NpgsqlCommand("SELECT column_name, data_type, is_nullable FROM information_schema.columns WHERE table_name = 'orders'", conn);
        var reader = cmd.ExecuteReader();
        while (reader.Read()) Console.WriteLine("$"{reader[0]} - {reader[1]}");
    }
}
