const fs = require('fs');
let js = fs.readFileSync('Frontend/pages/orders.js', 'utf-8');

js = js.replace(/async async/g, 'async');

fs.writeFileSync('Frontend/pages/orders.js', js, 'utf-8');
console.log("Fixed async async error");
