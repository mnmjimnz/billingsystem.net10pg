const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.Application/Services/SaleService.cs', 'utf-8');

code = code.replace(
    'NewStock = product.Stock - detail.Quantity,',
    'NewStock = await _productRepo.GetStockForBranchAsync(detail.ProductId, sale.BranchId),'
);

fs.writeFileSync('Backend/BillingSystem.Application/Services/SaleService.cs', code, 'utf-8');
console.log("Updated SaleService NewStock");
