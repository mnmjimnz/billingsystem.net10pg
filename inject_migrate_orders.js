const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.API/Program.cs', 'utf-8');
const sql = fs.readFileSync('Backend/Database/migration_orders.sql', 'utf-8').replace(/"/g, '\\"');

const endpoint = `
app.MapGet("/migrate-orders", async (DbConnectionFactory factory) => {
    try {
        using var connection = factory.CreateConnection();
        var sql = @"${sql}";
        await Dapper.SqlMapper.ExecuteAsync(connection, sql);
        return Results.Ok("Orders Migration successful");
    } catch (Exception ex) {
        return Results.Problem(ex.ToString());
    }
});
`;

code = code.replace('app.Run();', endpoint + '\napp.Run();');
fs.writeFileSync('Backend/BillingSystem.API/Program.cs', code, 'utf-8');
console.log("Injected /migrate-orders endpoint");
