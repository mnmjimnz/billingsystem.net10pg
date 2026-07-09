const fs = require('fs');

let html = fs.readFileSync('Frontend/pages/branches.html', 'utf-8');

const target = `<div class="mb-3">
                            <label class="form-label text-secondary small fw-semibold">Dirección</label>
                            <input type="text" class="form-control" id="branchAddress">
                        </div>`;

const newMap = `<div class="mb-3">
                            <label class="form-label text-secondary small fw-semibold">Dirección</label>
                            <input type="text" class="form-control" id="branchAddress">
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

if (html.includes(target) && !html.includes('branchLatitude')) {
    html = html.replace(target, newMap);
    // Make modal larger
    html = html.replace('<div class="modal-dialog modal-dialog-centered">', '<div class="modal-dialog modal-lg modal-dialog-centered">');
    fs.writeFileSync('Frontend/pages/branches.html', html, 'utf-8');
    console.log("Injected map and lat/lng into branches.html");
} else {
    console.log("Could not find target or already injected");
}
