const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.Infrastructure/Repositories/ProductRepository.cs', 'utf-8');

// For UpdateStockForBranchAsync
const updateGlobalStockSql = `
            UPDATE Products SET Stock = Stock + @QuantityChange, UpdatedAt = CURRENT_TIMESTAMP WHERE Id = @ProductId;
`;

code = code.replace(
    'DO UPDATE SET Stock = ProductStocks.Stock + @QuantityChange, UpdatedAt = CURRENT_TIMESTAMP;\n        ";',
    'DO UPDATE SET Stock = ProductStocks.Stock + @QuantityChange, UpdatedAt = CURRENT_TIMESTAMP;\n' + updateGlobalStockSql + '        ";'
);

// For UpdateStockAndCostForBranchAsync
code = code.replace(
    'var sql1 = "UPDATE Products SET Cost = @NewCost, UpdatedAt = CURRENT_TIMESTAMP WHERE Id = @Id";',
    'var sql1 = "UPDATE Products SET Cost = @NewCost, Stock = Stock + @QuantityChange, UpdatedAt = CURRENT_TIMESTAMP WHERE Id = @Id";'
);
// We also need to pass QuantityChange to sql1!
code = code.replace(
    'await connection.ExecuteAsync(sql1, new { NewCost = newCost, Id = productId }, transaction);',
    'await connection.ExecuteAsync(sql1, new { NewCost = newCost, Id = productId, QuantityChange = quantityChange }, transaction);'
);

fs.writeFileSync('Backend/BillingSystem.Infrastructure/Repositories/ProductRepository.cs', code, 'utf-8');
console.log("Updated ProductRepository to sync global stock");
