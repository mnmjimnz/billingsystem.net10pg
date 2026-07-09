const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.Application/Services/PurchaseService.cs', 'utf-8');

code = code.replace(
    'await _productRepo.UpdateStockAndCostAsync(detail.ProductId, detail.Quantity, detail.UnitCost);',
    'await _productRepo.UpdateStockAndCostForBranchAsync(detail.ProductId, purchase.BranchId, detail.Quantity, detail.UnitCost);'
);

code = code.replace(
    'ReferenceId = purchaseId,',
    'ReferenceId = purchaseId,\n                    BranchId = purchase.BranchId,'
);

// We should also calculate previous stock and new stock per branch
// But wait, the original code used `product.Stock`. We can keep `product.Stock` for now or calculate it correctly.
// Let's replace product.Stock with GetStockForBranchAsync
code = code.replace(
    'PreviousStock = product.Stock,',
    'PreviousStock = await _productRepo.GetStockForBranchAsync(detail.ProductId, purchase.BranchId) - detail.Quantity,' // because we updated it already! Wait, no, if we updated it already, the current branch stock is NewStock. Previous is NewStock - Quantity.
);

fs.writeFileSync('Backend/BillingSystem.Application/Services/PurchaseService.cs', code, 'utf-8');
console.log("Updated PurchaseService");
