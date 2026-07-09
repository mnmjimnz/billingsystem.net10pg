const fs = require('fs');

// 1. Fix branches.html
let html = fs.readFileSync('Frontend/pages/branches.html', 'utf-8');

if (!html.includes('leaflet.css')) {
    html = html.replace('<link rel="stylesheet" href="../assets/css/devextreme-theme.css?v=20260627004119">', 
        '<link rel="stylesheet" href="../assets/css/devextreme-theme.css?v=20260627004119">\n    <link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css" />\n    <style>#branch-map { height: 350px; width: 100%; border-radius: 8px; }</style>');
}

if (!html.includes('leaflet.js')) {
    html = html.replace('<script src="../assets/js/theme.js?v=20260627004119"></script>', 
        '<script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"></script>\n    <script src="../assets/js/theme.js?v=20260627004119"></script>');
}

const oldInputs = `<div class="row g-3 mb-3">
                                <div class="col-md-6">
                                    <div class="form-floating">
                                        <input type="number" step="any" class="form-control" id="branchLatitude">
                                        <label>Latitud (Opcional)</label>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-floating">
                                        <input type="number" step="any" class="form-control" id="branchLongitude">
                                        <label>Longitud (Opcional)</label>
                                    </div>
                                </div>
                            </div>`;

const newMap = `<div class="mb-3">
                                <div class="d-flex justify-content-between align-items-center mb-2">
                                    <label class="form-label text-secondary fw-bold mb-0">Ubicación en el Mapa</label>
                                    <button type="button" class="btn btn-sm btn-outline-primary" onclick="useMyLocation('branch')">
                                        <i class="bi bi-crosshair"></i> Mi Ubicación
                                    </button>
                                </div>
                                <div id="branch-map"></div>
                                <input type="hidden" id="branchLatitude">
                                <input type="hidden" id="branchLongitude">
                            </div>`;

if (html.includes(oldInputs)) {
    html = html.replace(oldInputs, newMap);
} else if (!html.includes('branch-map')) {
    // Just in case oldInputs was somehow different
    console.log("Could not find oldInputs in branches.html");
}

// Make modal larger
html = html.replace('<div class="modal-dialog modal-dialog-centered">', '<div class="modal-dialog modal-lg modal-dialog-centered">');

fs.writeFileSync('Frontend/pages/branches.html', html, 'utf-8');


// 2. Fix branches.js
let js = fs.readFileSync('Frontend/pages/branches.js', 'utf-8');

const mapLogic = `
let branchMap;
let branchMarker;

function initBranchMap() {
    if (branchMap) return; // already init
    branchMap = L.map('branch-map').setView([14.6349, -90.5069], 13);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png').addTo(branchMap);
    
    branchMarker = L.marker([14.6349, -90.5069], { draggable: true }).addTo(branchMap);
    
    branchMarker.on('dragend', function (e) {
        const coords = e.target.getLatLng();
        document.getElementById('branchLatitude').value = coords.lat.toFixed(6);
        document.getElementById('branchLongitude').value = coords.lng.toFixed(6);
    });

    branchMap.on('click', function (e) {
        branchMarker.setLatLng(e.latlng);
        document.getElementById('branchLatitude').value = e.latlng.lat.toFixed(6);
        document.getElementById('branchLongitude').value = e.latlng.lng.toFixed(6);
    });
}

document.getElementById('branchModal').addEventListener('shown.bs.modal', function () {
    initBranchMap();
    setTimeout(() => {
        branchMap.invalidateSize();
        const lat = document.getElementById('branchLatitude').value;
        const lng = document.getElementById('branchLongitude').value;
        if (lat && lng) {
            branchMap.setView([lat, lng], 16);
            branchMarker.setLatLng([lat, lng]);
        }
    }, 100);
});

window.useMyLocation = function(type) {
    if ("geolocation" in navigator) {
        navigator.geolocation.getCurrentPosition(function(position) {
            const lat = position.coords.latitude;
            const lng = position.coords.longitude;
            if (type === 'branch' && branchMap) {
                branchMap.setView([lat, lng], 16);
                branchMarker.setLatLng([lat, lng]);
                document.getElementById('branchLatitude').value = lat;
                document.getElementById('branchLongitude').value = lng;
                showToast("Ubicación obtenida.", "success");
            }
        }, function(error) {
            showToast("No se pudo obtener la ubicación. Permisos denegados.", "error");
        });
    } else {
        showToast("Geolocalización no soportada en este navegador.", "error");
    }
}
`;

if (!js.includes('branchMap')) {
    js += '\n' + mapLogic;
    fs.writeFileSync('Frontend/pages/branches.js', js, 'utf-8');
}

console.log("Fixed branches.html and branches.js");
