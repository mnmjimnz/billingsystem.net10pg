const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.Api/Program.cs', 'utf-8');

code = code.replace(
    'builder.Services.AddScoped<IBranchMovementRepository, BranchMovementRepository>();',
    'builder.Services.AddScoped<IBranchMovementRepository, BranchMovementRepository>();\nbuilder.Services.AddScoped<IStockTransferRepository, StockTransferRepository>();'
);

code = code.replace(
    'builder.Services.AddScoped<BillingSystem.Application.Interfaces.IBranchMovementService, BillingSystem.Application.Services.BranchMovementService>();',
    'builder.Services.AddScoped<BillingSystem.Application.Interfaces.IBranchMovementService, BillingSystem.Application.Services.BranchMovementService>();\nbuilder.Services.AddScoped<BillingSystem.Application.Interfaces.IStockTransferService, BillingSystem.Application.Services.StockTransferService>();'
);

fs.writeFileSync('Backend/BillingSystem.Api/Program.cs', code, 'utf-8');
console.log("Registered StockTransfer in Program.cs");
