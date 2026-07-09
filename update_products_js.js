const fs = require('fs');
let js = fs.readFileSync('Frontend/pages/products.js', 'utf8');

// 1. Add Image rendering in the table
js = js.replace(
    '<td class="ps-4">#${p.id}</td>\n                    <td>${p.barcode}</td>',
    '<td class="ps-4">#${p.id}</td>\n                    <td>${p.imageUrl ? `<img src="https://billingsystem-net10pg.onrender.com${p.imageUrl}" width="40" height="40" class="rounded object-fit-cover">` : `<div class="bg-light text-secondary rounded d-flex align-items-center justify-content-center" style="width:40px; height:40px; font-size:10px;">N/A</div>`}</td>\n                    <td>${p.barcode}</td>'
);

// 2. Add Upload button in actions
js = js.replace(
    '<button class="btn btn-sm btn-outline-info me-1 rounded-circle" onclick=\'showBarcodes(${JSON.stringify(p)})\' title="Ver CÃ³digos"><i class="bi bi-upc-scan"></i></button>',
    '<button class="btn btn-sm btn-outline-info me-1 rounded-circle" onclick=\'showBarcodes(${JSON.stringify(p)})\' title="Ver CÃ³digos"><i class="bi bi-upc-scan"></i></button>\n                          <button class="btn btn-sm btn-outline-warning me-1 rounded-circle" onclick=\'openUploadModal(${JSON.stringify(p)})\' title="Subir Imagen"><i class="bi bi-image"></i></button>'
);

// 3. Add modal and upload logic at the end
const uploadLogic = `
let uploadModalInstance;

document.addEventListener('DOMContentLoaded', () => {
    // Add to existing DOMContentLoaded if possible, or just define it here to initialize later
    setTimeout(() => {
        const modalEl = document.getElementById('uploadImageModal');
        if (modalEl) {
            uploadModalInstance = new bootstrap.Modal(modalEl);
        }
    }, 1000);
});

function openUploadModal(product) {
    document.getElementById('uploadImageProductId').value = product.id;
    const preview = document.getElementById('imagePreview');
    if (product.imageUrl) {
        preview.src = "https://billingsystem-net10pg.onrender.com" + product.imageUrl;
        preview.style.display = 'inline-block';
    } else {
        preview.src = '';
        preview.style.display = 'none';
    }
    document.getElementById('productImageFile').value = '';
    
    if(!uploadModalInstance) {
        uploadModalInstance = new bootstrap.Modal(document.getElementById('uploadImageModal'));
    }
    uploadModalInstance.show();
}

async function uploadProductImage() {
    const productId = document.getElementById('uploadImageProductId').value;
    const fileInput = document.getElementById('productImageFile');
    
    if (fileInput.files.length === 0) {
        showToast("Por favor selecciona una imagen.", "error");
        return;
    }

    const formData = new FormData();
    formData.append("file", fileInput.files[0]);

    try {
        const token = localStorage.getItem('token');
        const response = await fetch(\`https://billingsystem-net10pg.onrender.com/api/Products/\${productId}/image\`, {
            method: 'POST',
            headers: {
                'Authorization': \`Bearer \${token}\`
            },
            body: formData
        });

        if (response.ok) {
            showToast("Imagen subida correctamente", "success");
            uploadModalInstance.hide();
            await loadProducts(currentPage);
        } else {
            const err = await response.text();
            showToast("Error subiendo imagen: " + err, "error");
        }
    } catch (e) {
        console.error(e);
        showToast("Error de conexión al subir la imagen", "error");
    }
}
`;

js = js + uploadLogic;

fs.writeFileSync('Frontend/pages/products.js', js);
console.log("Updated products.js");
