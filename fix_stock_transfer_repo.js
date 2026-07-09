const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.Infrastructure/Repositories/StockTransferRepository.cs', 'utf-8');

code = code.replace(/u\.Name as UserName/g, 'u.FullName as UserName');

fs.writeFileSync('Backend/BillingSystem.Infrastructure/Repositories/StockTransferRepository.cs', code, 'utf-8');
console.log("Fixed u.Name -> u.FullName in StockTransferRepository");
