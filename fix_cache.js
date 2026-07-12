const fs = require('fs');

const ts = Date.now();

// 1. orders.html
let ordersHtml = fs.readFileSync('Frontend/pages/orders.html', 'utf8');
ordersHtml = ordersHtml.replace(/orders\.js(\?v=[0-9]+)?/g, `orders.js?v=${ts}`);
fs.writeFileSync('Frontend/pages/orders.html', ordersHtml);

// 2. store/index.html and store/cart.html
const storePages = ['Frontend/store/index.html', 'Frontend/store/cart.html'];
storePages.forEach(p => {
    let html = fs.readFileSync(p, 'utf8');
    html = html.replace(/app\.js(\?v=[0-9]+)?/g, `app.js?v=${ts}`);
    fs.writeFileSync(p, html);
});

console.log("Bumped cache versions.");
