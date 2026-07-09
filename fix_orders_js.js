const fs = require('fs');
let text = fs.readFileSync('Frontend/pages/orders.js', 'utf8');
let lines = text.split('\n');
lines.splice(35, 2); // 35 is index of line 36 (0-indexed). Remove 2 lines.
fs.writeFileSync('Frontend/pages/orders.js', lines.join('\n'));
console.log("Lines removed");
