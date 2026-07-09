const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.API/Controllers/ProductsController.cs', 'utf-8');

// Remove the dangling route
code = code.replace(/\[HttpGet\("barcode\/\{barcode\}"\)\]\s+\[HttpGet\("\{id\}\/stock"\)\]/, '[HttpGet("{id}/stock")]');

fs.writeFileSync('Backend/BillingSystem.API/Controllers/ProductsController.cs', code, 'utf-8');
console.log("Fixed routing in ProductsController");
