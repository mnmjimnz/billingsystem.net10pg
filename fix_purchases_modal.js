const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/purchases.html', 'utf-8');

// 1. Remove desktop-checkout-parent
html = html.replace(/<div class="card-body p-3 border-bottom bg-light bg-opacity-50 d-none d-lg-block" id="desktop-checkout-parent">[\s\S]*?<\/div>\s*<\/div>/, '');

// 2. We need to grab the checkout-form-container and put it inside the modal always.
// Actually, it's already inside mobile-checkout-modal? No, mobile-checkout-modal is just an offcanvas.
// Let's see what mobile-checkout-modal is in purchases.html
