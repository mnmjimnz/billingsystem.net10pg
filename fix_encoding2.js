const fs = require('fs');

function fixFile(path) {
    try {
        let text = fs.readFileSync(path, 'utf-8');
        text = text.replace(/Direcci\uFFFDn/g, 'Dirección');
        text = text.replace(/Selecci\uFFFDn/g, 'Selección');
        text = text.replace(/Ubicaci\uFFFDn/g, 'Ubicación');
        text = text.replace(/Sesi\uFFFDn/g, 'Sesión');
        text = text.replace(/Configuraci\uFFFDn/g, 'Configuración');
        text = text.replace(/Informaci\uFFFDn/g, 'Información');
        text = text.replace(/Acci\uFFFDn/g, 'Acción');
        text = text.replace(/M\uFFFDs/g, 'Más');
        text = text.replace(/A\uFFFDadir/g, 'Añadir');
        text = text.replace(/Tama\uFFFDos/g, 'Tamaños');
        text = text.replace(/Geolocalizaci\uFFFDn/g, 'Geolocalización');
        
        fs.writeFileSync(path, text, 'utf-8');
        console.log("Fixed " + path);
    } catch(e) {
        console.error(e);
    }
}

fixFile('Frontend/pages/orders.html');
fixFile('Frontend/pages/orders.js');
fixFile('Frontend/pages/branches.js');
fixFile('Frontend/pages/branches.html');
