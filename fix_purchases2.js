const fs = require('fs');

let html = fs.readFileSync('Frontend/pages/purchases.html', 'utf-8');

html = html.replace('<div class="card-body p-0" style="max-height: calc(100vh - 480px); overflow-y: auto;">', '<div class="card-body p-0 cart-items-container">');

fs.writeFileSync('Frontend/pages/purchases.html', html, 'utf-8');

let css = fs.readFileSync('Frontend/assets/css/devextreme-theme.css', 'utf-8');

css += `
.cart-items-container {
    overflow-y: auto;
    flex: 1 1 auto;
}
@media (min-width: 992px) {
    .cart-items-container {
        max-height: calc(100vh - 480px);
    }
}
@media (max-width: 991px) {
    .cart-items-container {
        max-height: none !important;
    }
}
`;

fs.writeFileSync('Frontend/assets/css/devextreme-theme.css', css, 'utf-8');
