const fs = require('fs');
let code = fs.readFileSync('Frontend/pages/products.js', 'utf-8');

code = code.replace(/\$\{s\.branchName\}/g, '${s.branchname || s.branchName}');

fs.writeFileSync('Frontend/pages/products.js', code, 'utf-8');
console.log("Fixed branchName to branchname in products.js");
