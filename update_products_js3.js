const fs = require('fs');
let js = fs.readFileSync('Frontend/pages/products.js', 'utf8');

js = js.replace(/let uploadModalInstance;[\s\S]*\}\),\ 1000\);\n\}\);/, '');

fs.writeFileSync('Frontend/pages/products.js', js);
console.log("Cleaned products.js");
