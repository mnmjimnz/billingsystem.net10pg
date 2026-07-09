const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/branches.html', 'utf-8');

html = html.replace('<div class="modal-dialog modal-dialog-centered">', '<div class="modal-dialog modal-lg modal-dialog-centered">');

if (!html.includes('leaflet.css')) {
    html = html.replace('<link rel="stylesheet" href="../assets/css/devextreme-theme.css?v=20260627004119">', 
        '<link rel="stylesheet" href="../assets/css/devextreme-theme.css?v=20260627004119">\n    <link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css" />');
}

if (!html.includes('leaflet.js')) {
    html = html.replace('<script src="../assets/js/theme.js?v=20260627004119"></script>', 
        '<script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"></script>\n    <script src="../assets/js/theme.js?v=20260627004119"></script>');
}

fs.writeFileSync('Frontend/pages/branches.html', html, 'utf-8');
console.log("Branches modal size and scripts updated");
