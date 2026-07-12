const fs = require('fs');
let html = fs.readFileSync('Frontend/store/cart.html', 'utf8');
const ts = Date.now();
html = html.replace(/app\.js\?v=[0-9]+/g, `app.js?v=${ts}`);
fs.writeFileSync('Frontend/store/cart.html', html);
console.log("bumped");
