const fs = require('fs');
let html = fs.readFileSync('Frontend/store/cart.html', 'utf8');

html = html.replace(/background:\s*'#343a40'/g, `background: '#fff'`);
html = html.replace(/color:\s*'#fff'/g, `color: '#212529'`);

fs.writeFileSync('Frontend/store/cart.html', html);
console.log("Fixed sweetalerts in cart");
