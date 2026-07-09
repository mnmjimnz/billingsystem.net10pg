const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/purchase-history.html', 'utf-8');

html = html.replace('<th>Total</th>', '<th>Total</th>\n                                          <th class="text-center">Acciones</th>');

fs.writeFileSync('Frontend/pages/purchase-history.html', html, 'utf-8');
