const fs = require('fs');

let js = fs.readFileSync('Frontend/pages/stock-transfers.js', 'utf8');
js = js.replace('initSidebar();\n', '');
js = js.replace('initSidebar();\r\n', '');

let funcStart = js.indexOf('function initSidebar() {');
if (funcStart !== -1) {
    let funcEnd = js.indexOf('}', js.indexOf('} catch', funcStart)) + 1;
    if (funcEnd > 0) {
        js = js.substring(0, funcStart) + js.substring(funcEnd);
    }
}
fs.writeFileSync('Frontend/pages/stock-transfers.js', js);
console.log("Removed initSidebar from stock-transfers.js");
