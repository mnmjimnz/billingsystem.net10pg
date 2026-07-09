const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/branches.html', 'utf-8');

const searchTarget = `                        <div class="mb-3">
                            <label class="form-label text-secondary small fw-semibold">Teléfono</label>
                            <input type="text" class="form-control" id="branchPhone">
                        </div>`;

const injectTarget = `                        <div class="mb-3">
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
    html = html.replace('<div class="modal-dialog modal-dialog-centered">', '<div class="modal-dialog modal-lg modal-dialog-centered">');
    fs.writeFileSync('Frontend/pages/branches.html', html, 'utf-8');
    console.log("SUCCESS");
} else {
    console.log("FAILED to find searchTarget or branchLatitude already exists");
}
