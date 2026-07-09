const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/branches.html', 'utf-8');

const searchTarget = `<div class="mb-3">
                            <label class="form-label text-secondary small fw-semibold">Teléfono</label>
                            <input type="text" class="form-control" id="branchPhone">
                        </div>`;

const injectTarget = `<div class="mb-3">
                            <label class="form-label text-secondary small fw-semibold">Teléfono</label>
                            <input type="text" class="form-control" id="branchPhone">
                        </div>
                        <div class="mb-3">
                            <div class="d-flex justify-content-between align-items-center mb-2">
                                <label class="form-label text-secondary fw-bold mb-0">Ubicación en el Mapa</label>
                                <button type="button" class="btn btn-sm btn-outline-primary" onclick="useMyLocation('branch')">
                                    <i class="bi bi-crosshair"></i> Mi Ubicación
                                </button>
                            </div>
                            <div id="branch-map" style="height: 350px; width: 100%; border-radius: 8px;"></div>
                            <input type="hidden" id="branchLatitude">
                            <input type="hidden" id="branchLongitude">
                        </div>`;

if (html.includes(searchTarget) && !html.includes('branchLatitude')) {
    html = html.replace(searchTarget, injectTarget);
    if (!html.includes('modal-lg')) {
        html = html.replace('<div class="modal-dialog modal-dialog-centered">', '<div class="modal-dialog modal-lg modal-dialog-centered">');
    }
    
    if (!html.includes('leaflet.css')) {
        html = html.replace('<link rel="stylesheet" href="../assets/css/devextreme-theme.css?v=20260627004119">', 
            '<link rel="stylesheet" href="../assets/css/devextreme-theme.css?v=20260627004119">\n    <link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css" />');
    }

    if (!html.includes('leaflet.js')) {
        html = html.replace('<script src="../assets/js/theme.js?v=20260627004119"></script>', 
            '<script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"></script>\n    <script src="../assets/js/theme.js?v=20260627004119"></script>');
    }
    
    fs.writeFileSync('Frontend/pages/branches.html', html, 'utf-8');
    console.log("SUCCESS");
} else {
    console.log("FAILED or already injected");
}
