const fs = require('fs');

// 1. Fix HTML
let html = fs.readFileSync('Frontend/pages/purchases.html', 'utf-8');
html = html.replace(
    '<div class="col-lg-4" id="cart-panel">',
    '<div class="col-lg-4 h-100" id="cart-panel">'
);
html = html.replace(
    '<div class="card border-info shadow-sm" style="border-width: 2px;">',
    '<div class="card border-info shadow-sm d-flex flex-column h-100" style="border-width: 2px;">'
);
fs.writeFileSync('Frontend/pages/purchases.html', html, 'utf-8');

// 2. Fix CSS
let css = fs.readFileSync('Frontend/assets/css/devextreme-theme.css', 'utf-8');
css = css.replace(
    '          width: 100%;\n          z-index: 1040;',
    '          width: 100%;\n          height: calc(100vh - 60px);\n          z-index: 1040;'
);
css = css.replace(
    '#cart-panel > .card {\n          height: 100%;\n          border: none !important;',
    '#cart-panel > .card {\n          height: 100%;\n          display: flex;\n          flex-direction: column;\n          border: none !important;'
);
// Also remove the explicit max-heights since flex-column h-100 will handle it beautifully
css = css.replace(
    'max-height: calc(100vh - 480px);',
    '/* max-height handled by flex */'
);
css = css.replace(
    'max-height: calc(100vh - 380px);',
    '/* max-height handled by flex */'
);
fs.writeFileSync('Frontend/assets/css/devextreme-theme.css', css, 'utf-8');
