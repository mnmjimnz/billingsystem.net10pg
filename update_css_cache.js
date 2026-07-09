const fs = require('fs');
const path = require('path');
const dir = 'Frontend/pages';

const files = fs.readdirSync(dir).filter(f => f.endsWith('.html'));
for (const file of files) {
    const filePath = path.join(dir, file);
    let html = fs.readFileSync(filePath, 'utf8');
    
    // Replace theme CSS cache query
    html = html.replace(/devextreme-theme\.css\?v=[0-9]+/g, `devextreme-theme.css?v=${Date.now()}`);
    
    fs.writeFileSync(filePath, html);
}
console.log("Updated cache strings.");
