const fs = require('fs');
let js = fs.readFileSync('Frontend/pages/products.js', 'utf8');

// Update clearForm
js = js.replace(
    "document.getElementById('modalTitle').innerText = 'Nuevo Producto';",
    "document.getElementById('modalTitle').innerText = 'Nuevo Producto';\n    document.getElementById('productImageFile').value = '';\n    document.getElementById('imagePreview').style.display = 'none';"
);

// Update editProduct
js = js.replace(
    "document.getElementById('modalTitle').innerText = 'Editar Producto';",
    "document.getElementById('modalTitle').innerText = 'Editar Producto';\n    document.getElementById('productImageFile').value = '';\n    const preview = document.getElementById('imagePreview');\n    if (product.imageUrl) {\n        preview.src = 'https://billingsystem-net10pg.onrender.com' + product.imageUrl;\n        preview.style.display = 'inline-block';\n    } else {\n        preview.src = '';\n        preview.style.display = 'none';\n    }"
);

// Update saveProduct
js = js.replace(
    /if \(id\) \{\s*await ApiClient\.request\(`\/Products\/\$\{id\}`,\s*'PUT',\s*product\);\s*\} else \{\s*await ApiClient\.request\('\/Products',\s*'POST',\s*product\);\s*\}/,
    `let savedId = id;
        if (id) {
            await ApiClient.request(\`/Products/\${id}\`, 'PUT', product);
        } else {
            const result = await ApiClient.request('/Products', 'POST', product);
            savedId = result.id;
        }

        // Check if there is an image to upload
        const fileInput = document.getElementById('productImageFile');
        if (fileInput.files.length > 0 && savedId) {
            const formData = new FormData();
            formData.append("file", fileInput.files[0]);
            const token = localStorage.getItem('token');
            await fetch(\`https://billingsystem-net10pg.onrender.com/api/Products/\${savedId}/image\`, {
                method: 'POST',
                headers: { 'Authorization': \`Bearer \${token}\` },
                body: formData
            });
        }`
);

// Remove the openUploadModal function and uploadProductImage logic
js = js.replace(/function openUploadModal[\s\S]*?\}\n\}/, '');

// Also remove the extra button from the table rendering
js = js.replace(
    '<button class="btn btn-sm btn-outline-warning me-1 rounded-circle" onclick=\'openUploadModal(${JSON.stringify(p)})\' title="Subir Imagen"><i class="bi bi-image"></i></button>',
    ''
);

fs.writeFileSync('Frontend/pages/products.js', js);
console.log("Updated products.js");
