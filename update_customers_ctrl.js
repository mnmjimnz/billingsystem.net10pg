const fs = require('fs');
let repo = fs.readFileSync('Backend/BillingSystem.Api/Controllers/CustomersController.cs', 'utf8');
repo = repo.replace('using BillingSystem.Infrastructure.Repositories;', 'using BillingSystem.Domain.Interfaces;\nusing BillingSystem.Infrastructure.Repositories;');
fs.writeFileSync('Backend/BillingSystem.Api/Controllers/CustomersController.cs', repo);
console.log("Updated CustomersController");
