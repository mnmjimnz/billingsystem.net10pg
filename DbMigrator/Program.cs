using System;
using System.IO;
using Npgsql;

var connectionString = "Host=dpg-d8vej7p9rddc73c9e6jg-a.oregon-postgres.render.com;Port=5432;Database=billing_system_dtum;Username=admin;Password=3v59bzZTlJeh4Kfc3AohbBcYU63Y6h86";
var sqlFile = args.Length > 0 ? args[0] : throw new Exception("Please provide a SQL file path.");
var sql = File.ReadAllText(sqlFile);

Console.WriteLine($"Executing Migration from {sqlFile}...");
using var conn = new NpgsqlConnection(connectionString);
conn.Open();
using var cmd = new NpgsqlCommand(sql, conn);
cmd.ExecuteNonQuery();
Console.WriteLine("Migration completed successfully.");
