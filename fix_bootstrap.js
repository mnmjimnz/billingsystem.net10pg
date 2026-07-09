const fs = require('fs');
const path = require('path');

const pagesDir = 'Frontend/pages';
const files = fs.readdirSync(pagesDir).filter(f => f.endsWith('.html'));

files.forEach(file => {
    const filePath = path.join(pagesDir, file);
    let html = fs.readFileSync(filePath, 'utf-8');
    
    if (!html.includes('bootstrap.min.css')) {
        html = html.replace(
            '<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">',
            '<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet">\n    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">'
        );
        fs.writeFileSync(filePath, html, 'utf-8');
        console.log(`Added Bootstrap to ${file}`);
    }
});
