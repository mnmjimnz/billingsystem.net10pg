const fs = require('fs');
const path = require('path');

const pagesDir = 'Frontend/pages';
const files = fs.readdirSync(pagesDir).filter(f => f.endsWith('.html'));

files.forEach(file => {
    let html = fs.readFileSync(path.join(pagesDir, file), 'utf-8');
    const regex = /<link href="https:\/\/cdn\.jsdelivr\.net\/npm\/bootstrap@5\.3\.2\/dist\/css\/bootstrap\.min\.css" rel="stylesheet">\s*/g;
    if (regex.test(html)) {
        html = html.replace(regex, '');
        fs.writeFileSync(path.join(pagesDir, file), html, 'utf-8');
        console.log(`Removed bootstrap from ${file}`);
    }
});
