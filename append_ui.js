const fs = require('fs');

const uiCorePath = 'Frontend/assets/js/ui-core.js';
let uiCore = fs.readFileSync(uiCorePath, 'utf-8');

const geoCodeFn = `
window.updateAddressFromCoords = async function(lat, lng, inputId) {
    try {
        const response = await fetch(\`https://nominatim.openstreetmap.org/reverse?format=json&lat=\${lat}&lon=\${lng}&zoom=18&addressdetails=1\`, {
            headers: { 'Accept-Language': 'es' }
        });
        const data = await response.json();
        if (data && data.display_name) {
            const el = document.getElementById(inputId);
            if (el) el.value = data.display_name;
        }
    } catch (e) {
        console.error("Geocoding error:", e);
    }
};
`;

if (!uiCore.includes('updateAddressFromCoords')) {
    fs.writeFileSync(uiCorePath, uiCore + '\n' + geoCodeFn, 'utf-8');
    console.log("Appended updateAddressFromCoords to ui-core.js");
}
