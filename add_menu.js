const fs = require('fs');
let menu = fs.readFileSync('Frontend/assets/js/menu.js', 'utf-8');

const newMenuItem = `
    {
        name: 'Pedidos y Entregas',
        icon: 'bi-box-seam',
        url: '/pages/orders.html',
        permission: 'MANAGE_ORDERS'
    },`;

if (!menu.includes('MANAGE_ORDERS')) {
    menu = menu.replace('// Inventory & Supply', newMenuItem + '\n    // Inventory & Supply');
    fs.writeFileSync('Frontend/assets/js/menu.js', menu, 'utf-8');
    console.log("Added to menu.js");
}
