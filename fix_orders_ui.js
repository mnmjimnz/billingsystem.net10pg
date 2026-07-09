const fs = require('fs');

// 1. Fix orders.html
let html = fs.readFileSync('Frontend/pages/orders.html', 'utf-8');

// Change map height
html = html.replace('#order-map { height: 300px;', '#order-map { height: 450px;');

// Reorganize layout
const oldLayout = `<div class="row">
                        <div class="col-md-7 border-end pe-4">`;

html = html.replace(oldLayout, `<div>
                        <div class="mb-4">`);

const oldMapSection = `</form>
                        </div>
                        <div class="col-md-5 ps-4">
                            <h6 class="fw-bold text-secondary mb-3">Ubicación de Entrega (Mapa)</h6>
                            <p class="small text-muted mb-2">Haz clic en el mapa o arrastra el marcador para seleccionar las coordenadas exactas de entrega.</p>
                            <div id="order-map"></div>
                            <div class="row g-2">
                                <div class="col-6">
                                    <div class="form-floating">
                                        <input type="text" class="form-control form-control-sm" id="orderLat" readonly>
                                        <label>Latitud</label>
                                    </div>
                                </div>
                                <div class="col-6">
                                    <div class="form-floating">
                                        <input type="text" class="form-control form-control-sm" id="orderLng" readonly>
                                        <label>Longitud</label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>`;

const newMapSection = `</form>
                        </div>
                        <hr class="my-4">
                        <div class="mb-2">
                            <div class="d-flex justify-content-between align-items-end mb-3">
                                <div>
                                    <h6 class="fw-bold text-secondary mb-1"><i class="bi bi-geo-alt"></i> Ubicación de Entrega (Mapa)</h6>
                                    <p class="small text-muted mb-0">Haz clic en el mapa o arrastra el marcador para la entrega.</p>
                                </div>
                                <button type="button" class="btn btn-sm btn-outline-primary" onclick="useMyLocation('order')">
                                    <i class="bi bi-crosshair"></i> Usar Mi Ubicación
                                </button>
                            </div>
                            <div id="order-map"></div>
                            <input type="hidden" id="orderLat">
                            <input type="hidden" id="orderLng">
                        </div>
                    </div>`;

html = html.replace(oldMapSection, newMapSection);
fs.writeFileSync('Frontend/pages/orders.html', html, 'utf-8');


// 2. Fix orders.js
let js = fs.readFileSync('Frontend/pages/orders.js', 'utf-8');
const geoLogic = `
window.useMyLocation = function(type) {
    if ("geolocation" in navigator) {
        navigator.geolocation.getCurrentPosition(function(position) {
            const lat = position.coords.latitude;
            const lng = position.coords.longitude;
            if (type === 'order') {
                modalMap.setView([lat, lng], 16);
                modalMarker.setLatLng([lat, lng]);
                document.getElementById('orderLat').value = lat;
                document.getElementById('orderLng').value = lng;
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

if (!js.includes('useMyLocation')) {
    js += '\n' + geoLogic;
    fs.writeFileSync('Frontend/pages/orders.js', js, 'utf-8');
}

console.log("Fixed orders.html and orders.js");
