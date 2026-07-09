const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/purchases.html', 'utf-8');

html = html.replace(/N\ufffd de Factura/g, 'N° de Factura');
html = html.replace(/N de Factura/g, 'N° de Factura');

fs.writeFileSync('Frontend/pages/purchases.html', html, 'utf-8');
console.log("Fixed encoding issue in purchases.html");
