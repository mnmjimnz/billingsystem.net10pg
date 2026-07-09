const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.Application/Services/SaleService.cs', 'utf-8');

code = code.replace(
    'await _productRepo.UpdateStockAsync(detail.ProductId, -detail.Quantity);',
    'await _productRepo.UpdateStockForBranchAsync(detail.ProductId, sale.BranchId, -detail.Quantity);'
);

code = code.replace(
    'ReferenceId = saleId,',
    'ReferenceId = saleId,\n                    BranchId = sale.BranchId,'
);

code = code.replace(
    'PreviousStock = product.Stock,',
    'PreviousStock = await _productRepo.GetStockForBranchAsync(detail.ProductId, sale.BranchId) + detail.Quantity,' // Because we just reduced it, the previous was NewStock + Quantity
);

// We also need to add validation to check if branch stock is enough before selling!
// Let's find where we check stock.
// In SaleService.cs we might not be checking stock! Wait, let's see.

fs.writeFileSync('Backend/BillingSystem.Application/Services/SaleService.cs', code, 'utf-8');
console.log("Updated SaleService");
