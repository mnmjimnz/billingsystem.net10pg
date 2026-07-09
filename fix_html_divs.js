const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/purchases.html', 'utf-8');

html = html.replace(
    '                            </div>\n                            </div>\n                              </div>\n                              <div class="card-body p-0 cart-items-container">',
    '                            </div>\n                            </div>\n                              <div class="card-body p-0 cart-items-container">'
);

fs.writeFileSync('Frontend/pages/purchases.html', html, 'utf-8');
