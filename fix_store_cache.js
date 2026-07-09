const fs = require('fs');
const path = require('path');
const dir = 'Frontend/store';

const files = fs.readdirSync(dir).filter(f => f.endsWith('.html'));
const ts = Date.now();

for (const file of files) {
    const filePath = path.join(dir, file);
    let html = fs.readFileSync(filePath, 'utf8');
    
    // Replace app.js cache query if it exists or add it
    html = html.replace(/app\.js(\?v=[0-9]+)?/g, `app.js?v=${ts}`);
    
    // Replace style.css cache query if it exists or add it
    html = html.replace(/style\.css(\?v=[0-9]+)?/g, `style.css?v=${ts}`);
    
    fs.writeFileSync(filePath, html);
}
console.log("Updated cache strings for store.");
