const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/reports.html', 'utf-8');

html = html.replace('Mï¿½dulo', 'Módulo');
fs.writeFileSync('Frontend/pages/reports.html', html, 'utf-8');
