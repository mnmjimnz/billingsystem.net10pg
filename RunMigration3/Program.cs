using System;
using Npgsql;
using Dapper;

class Program
{
    static void Main()
    {
        var connStr = "Host=dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com;Port=5432;Database=billing_system_dtum;Username=admin;Password=3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86";
        
        using var conn = new NpgsqlConnection(connStr);
        conn.Open();

        var sqlAddColumn = @"ALTER TABLE Users ADD COLUMN IF NOT EXISTS IsAdmin BOOLEAN DEFAULT false;";
        conn.Execute(sqlAddColumn);
        Console.WriteLine("Added IsAdmin column to Users table.");

        var sqlSetAdmin = @"UPDATE Users SET IsAdmin = true WHERE RoleId = 1;";
        var rows = conn.Execute(sqlSetAdmin);
        Console.WriteLine($"Updated {rows} users to IsAdmin = true based on RoleId = 1.");
    }
}
