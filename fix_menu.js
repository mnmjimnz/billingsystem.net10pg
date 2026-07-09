const fs = require('fs');
let menu = fs.readFileSync('Frontend/assets/js/menu.js', 'utf-8');

const newMenuItem = `    { url: 'orders.html', icon: 'bi-signpost-split', text: 'Pedidos y Entregas', permission: 'MANAGE_ORDERS' },\n`;

if (!menu.includes('MANAGE_ORDERS')) {
    menu = menu.replace("    { url: 'stock-transfers.html'", newMenuItem + "    { url: 'stock-transfers.html'");
    fs.writeFileSync('Frontend/assets/js/menu.js', menu, 'utf-8');
    console.log("Properly added MANAGE_ORDERS to menu.js");
}
