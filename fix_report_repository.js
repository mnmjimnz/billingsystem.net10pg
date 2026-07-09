const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.Infrastructure/Repositories/ReportRepository.cs', 'utf-8');

// Replace p.Date with p.CreatedAt for Purchases
code = code.replace(/p\.Date/g, 'p.CreatedAt');

fs.writeFileSync('Backend/BillingSystem.Infrastructure/Repositories/ReportRepository.cs', code, 'utf-8');
console.log("Fixed Purchases p.Date -> p.CreatedAt in ReportRepository");
