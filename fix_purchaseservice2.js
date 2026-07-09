const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.Application/Services/PurchaseService.cs', 'utf-8');

code = code.replace(
    'NewStock = product.Stock + detail.Quantity,',
    'NewStock = await _productRepo.GetStockForBranchAsync(detail.ProductId, purchase.BranchId),'
);

fs.writeFileSync('Backend/BillingSystem.Application/Services/PurchaseService.cs', code, 'utf-8');
console.log("Updated PurchaseService NewStock");
