const fs = require('fs');
let js = fs.readFileSync('Frontend/pages/orders.js', 'utf-8');
js = js.replace(/∩┐╜/g, 'ó');
js = js.replace(/├│/g, 'ó');
fs.writeFileSync('Frontend/pages/orders.js', js, 'utf-8');
console.log("Fixed lingering chars");
