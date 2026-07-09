const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.Application/Services/PurchaseService.cs', 'utf-8');

code = code.replace(/using var scope = new TransactionScope\(TransactionScopeAsyncFlowOption\.Enabled\);\s*/, '');
code = code.replace(/scope\.Complete\(\);\s*/, '');

fs.writeFileSync('Backend/BillingSystem.Application/Services/PurchaseService.cs', code, 'utf-8');
console.log("Removed TransactionScope from PurchaseService");
