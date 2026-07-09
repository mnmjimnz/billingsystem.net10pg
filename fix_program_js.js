const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.Api/Program.cs', 'utf-8');

const migrationEndpoint = `
app.MapGet("/migrate-stocks", async (DbConnectionFactory factory) => {
    try {
        using var connection = factory.CreateConnection();
        var sql = @"
            CREATE TABLE IF NOT EXISTS ProductStocks (
                Id SERIAL PRIMARY KEY,
                ProductId INT NOT NULL REFERENCES Products(Id),
                BranchId INT NOT NULL REFERENCES Branches(Id),
                Stock INT NOT NULL DEFAULT 0,
                CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                UpdatedAt TIMESTAMP NULL,
                UNIQUE(ProductId, BranchId)
            );

            ALTER TABLE InventoryMovements ADD COLUMN IF NOT EXISTS BranchId INT NULL REFERENCES Branches(Id);

            CREATE TABLE IF NOT EXISTS StockTransfers (
                Id SERIAL PRIMARY KEY,
                ProductId INT NOT NULL REFERENCES Products(Id),
                FromBranchId INT NOT NULL REFERENCES Branches(Id),
                ToBranchId INT NOT NULL REFERENCES Branches(Id),
                Quantity INT NOT NULL,
                UserId INT NOT NULL REFERENCES Users(Id),
                CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                Notes TEXT NULL
            );

            INSERT INTO ProductStocks (ProductId, BranchId, Stock, CreatedAt, UpdatedAt)
            SELECT p.Id, (SELECT MIN(Id) FROM Branches), p.Stock, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
            FROM Products p
            WHERE NOT EXISTS (
                SELECT 1 FROM ProductStocks ps WHERE ps.ProductId = p.Id
            );

            UPDATE InventoryMovements 
            SET BranchId = (SELECT MIN(Id) FROM Branches) 
            WHERE BranchId IS NULL;
        ";
        await Dapper.SqlMapper.ExecuteAsync(connection, sql);
        return Results.Ok("Migration successful");
    } catch (Exception ex) {
        return Results.Problem(ex.ToString());
    }
});
`;

if (!code.includes('/migrate-stocks')) {
    code = code.replace('app.Run();', migrationEndpoint + '\napp.Run();');
    code = "using Dapper;\n" + code;
    fs.writeFileSync('Backend/BillingSystem.Api/Program.cs', code, 'utf-8');
    console.log("Added /migrate-stocks to Program.cs");
}
