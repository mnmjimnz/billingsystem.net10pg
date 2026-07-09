const fs = require('fs');

let repo = fs.readFileSync('Backend/BillingSystem.Infrastructure/Repositories/StockTransferRepository.cs', 'utf8');

repo = repo.replace('SELECT st.*, p.Name, fb.Name, tb.Name, u.FullName', 'SELECT st.*, p.Id, p.Name, fb.Id, fb.Name, tb.Id, tb.Name, u.Id, u.FullName');

fs.writeFileSync('Backend/BillingSystem.Infrastructure/Repositories/StockTransferRepository.cs', repo);
console.log("Fixed SQL");
