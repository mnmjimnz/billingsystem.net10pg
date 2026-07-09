const fs = require('fs');

function updateAuthFile(path) {
    let html = fs.readFileSync(path, 'utf8');

    // Replace hardcoded "Nexus Store" in the card header/brand
    html = html.replace(/<h3 class="fw-bold mb-4">Nexus Store<\/h3>/, '<h3 class="fw-bold mb-4" id="storeNameBrand">Tienda</h3>');
    html = html.replace(/<h3 class="fw-bold mb-4 text-center">Nexus Store<\/h3>/, '<h3 class="fw-bold mb-4 text-center" id="storeNameBrand">Tienda</h3>');
    
    // Sometimes it's inside an anchor or div
    html = html.replace(/>Nexus Store</g, '><span id="storeNameBrand">Tienda</span><');

    const fetchScript = `
    <script>
        document.addEventListener('DOMContentLoaded', async () => {
            try {
                const res = await fetch('https://billingsystem-net10pg.onrender.com/api/Settings');
                if (res.ok) {
                    const settings = await res.json();
                    if (settings && settings.companyName) {
                        const els = document.querySelectorAll('#storeNameBrand');
                        els.forEach(el => el.innerText = settings.companyName);
                        document.title = settings.companyName + ' - Autenticación';
                    }
                }
            } catch (e) {
                console.log("No se pudo cargar el nombre de la tienda");
            }
        });
    </script>
</body>`;

    if(!html.includes('api/Settings')) {
        html = html.replace(/<\/body>/, fetchScript);
    }
    
    fs.writeFileSync(path, html);
    console.log(path + " updated");
}

updateAuthFile('Frontend/store/login.html');
updateAuthFile('Frontend/store/register.html');
