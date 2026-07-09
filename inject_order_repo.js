const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.API/Program.cs', 'utf-8');

if (!code.includes('IOrderRepository')) {
    code = code.replace(
        'builder.Services.AddScoped<IStockTransferRepository, StockTransferRepository>();',
        'builder.Services.AddScoped<IStockTransferRepository, StockTransferRepository>();\nbuilder.Services.AddScoped<IOrderRepository, OrderRepository>();'
    );
    fs.writeFileSync('Backend/BillingSystem.API/Program.cs', code, 'utf-8');
    console.log("Injected IOrderRepository");
}
