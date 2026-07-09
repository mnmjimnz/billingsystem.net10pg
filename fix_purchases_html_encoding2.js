const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/purchases.html', 'utf-8');

html = html.replace(/N de Factura/g, 'Nº de Factura');
html = html.replace(/Crdito/g, 'Crédito');
html = html.replace(/N\? de Factura/g, 'Nº de Factura');
html = html.replace(/Cr\?dito/g, 'Crédito');
html = html.replace(/N\ufffd de Factura/g, 'Nº de Factura');
html = html.replace(/Cr\ufffddito/g, 'Crédito');

fs.writeFileSync('Frontend/pages/purchases.html', html, 'utf-8');
console.log("Fixed encoding issue in purchases.html AGAIN");
