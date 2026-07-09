const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.API/Program.cs', 'utf-8');

const newRoute = `app.MapGet("/add-permission-transfers", async (DbConnectionFactory factory) => {
    try {
        using var connection = factory.CreateConnection();
        // Insert permission if it doesn't exist
        var sql = @"
            INSERT INTO Permissions (Name, Description) 
            SELECT 'MANAGE_TRANSFERS', 'Gestionar Traslados'
            WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'MANAGE_TRANSFERS');
        ";
        await Dapper.SqlMapper.ExecuteAsync(connection, sql);
        return Results.Ok("Permission added");
    } catch (Exception ex) {
        return Results.Problem(ex.ToString());
    }
});

app.MapControllers();`;

code = code.replace('app.MapControllers();', newRoute);
fs.writeFileSync('Backend/BillingSystem.API/Program.cs', code, 'utf-8');
console.log("Added /add-permission-transfers route");
