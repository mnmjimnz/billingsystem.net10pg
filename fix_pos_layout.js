const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/pos.html', 'utf-8');
html = html.replace(
    '<div class="col-lg-4" id="cart-panel">',
    '<div class="col-lg-4 h-100" id="cart-panel">'
);
html = html.replace(
    '<div class="card border-primary shadow-sm" style="border-width: 2px;">',
    '<div class="card border-primary shadow-sm d-flex flex-column h-100" style="border-width: 2px;">'
);
fs.writeFileSync('Frontend/pages/pos.html', html, 'utf-8');
