const fs = require('fs');

let html = fs.readFileSync('Frontend/pages/orders.html', 'utf-8');
html = html.replace(/generar\uFFFD/g, 'generará');
html = html.replace(/autom\uFFFDticamente/g, 'automáticamente');
html = html.replace(/dar\uFFFD/g, 'dará');
html = html.replace(/Selecci\uFFFDn/g, 'Selección');
fs.writeFileSync('Frontend/pages/orders.html', html, 'utf-8');

let js = fs.readFileSync('Frontend/pages/orders.js', 'utf-8');
js = js.replace(/Funci\uFFFDn/g, 'Función');
js = js.replace(/\uFFFDptimamente/g, 'óptimamente');
js = js.replace(/ubicaci\uFFFDn/g, 'ubicación');
js = js.replace(/ser\uFFFD/g, 'será');
js = js.replace(/\uFFFD/g, '¿'); // For "¿Seguro que deseas"
fs.writeFileSync('Frontend/pages/orders.js', js, 'utf-8');

console.log("Fixed encoding again");
