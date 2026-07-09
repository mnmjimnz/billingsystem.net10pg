const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/purchases.html', 'utf-8');

html = html.replace('class="card-body p-0 cart-items-container"', 'class="card-body p-0 pos-cart-items-container"');

// also hide card-header on mobile
html = html.replace('<div class="card-header border-bottom pt-4 pb-3">', '<div class="card-header border-bottom pt-4 pb-3 d-none d-lg-block">');

fs.writeFileSync('Frontend/pages/purchases.html', html, 'utf-8');
