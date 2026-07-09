const fs = require('fs');
let js = fs.readFileSync('Frontend/pages/orders.js', 'utf-8');
js = js.replace(/A\uFFFDSeguro que deseas cancelar este pedido\? El stock reservado serA\uFFFD devuelto al inventario\./g, 
    '¿Seguro que deseas cancelar este pedido? El stock reservado será devuelto al inventario.');
js = js.replace(/ASeguro que deseas cancelar este pedido\? El stock reservado serA devuelto al inventario\./g, 
    '¿Seguro que deseas cancelar este pedido? El stock reservado será devuelto al inventario.');
fs.writeFileSync('Frontend/pages/orders.js', js, 'utf-8');
console.log("Fixed text in orders.js");
