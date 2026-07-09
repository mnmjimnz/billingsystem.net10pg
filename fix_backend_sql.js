const fs = require('fs');
let content = fs.readFileSync('Backend/BillingSystem.Infrastructure/Repositories/PurchaseRepository.cs', 'utf-8');
content = content.replace('pr.Code as ProductCode', 'pr.Barcode as ProductCode');
fs.writeFileSync('Backend/BillingSystem.Infrastructure/Repositories/PurchaseRepository.cs', content, 'utf-8');
console.log("Fixed SQL column name");
