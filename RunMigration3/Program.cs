using System;
using Npgsql;
using Dapper;

class Program
{
    static void Main()
    {
        var connStr = "Host=dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com;Port=5432;Database=billing_system_dtum;Username=admin;Password=3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86";
        var sql = "SELECT Id, Name, ImageUrl FROM Products ORDER BY Id DESC LIMIT 5;";
        using var conn = new NpgsqlConnection(connStr);
        conn.Open();
        var products = conn.Query(sql);
        foreach(var p in products) {
            Console.WriteLine($"ID: {p.id}, Name: {p.name}, ImageUrl: {p.imageurl}");
        }
    }
}
