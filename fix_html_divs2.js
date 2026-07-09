const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/purchases.html', 'utf-8');

// The exact string to replace might have \r\n
const oldStr = `                            </div>
                            </div>
                              </div>
                              <div class="card-body p-0 cart-items-container">`;
const newStr = `                            </div>
                            </div>
                              <div class="card-body p-0 cart-items-container">`;

if (html.includes(oldStr)) {
    console.log("Matched exactly!");
    html = html.replace(oldStr, newStr);
} else {
    console.log("Did not match, using regex");
    html = html.replace(/<\/div>\s*<\/div>\s*<\/div>\s*<div class="card-body p-0 cart-items-container">/, 
        `</div>\n                            </div>\n                              <div class="card-body p-0 cart-items-container">`);
}

fs.writeFileSync('Frontend/pages/purchases.html', html, 'utf-8');
