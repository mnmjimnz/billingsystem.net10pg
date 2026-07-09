const fs = require('fs');
let content = fs.readFileSync('Backend/BillingSystem.API/Controllers/PurchasesController.cs', 'utf-8');
content = content.replace(
    'await _purchaseService.GetPurchaseWithDetailsAsync(id);',
    'await _service.GetPurchaseWithDetailsAsync(id);'
);
fs.writeFileSync('Backend/BillingSystem.API/Controllers/PurchasesController.cs', content, 'utf-8');
