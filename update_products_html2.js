const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/products.html', 'utf8');

// Add file input inside productForm
const formContent = `
                <div class="form-floating">
                    <select class="form-select" id="productCategory" required>
                        <!-- Loaded dynamically -->
                    </select>
                    <label>CategorÃ­a</label>
                </div>
            
                  <div class="form-check mt-3 mb-2">
                      <input class="form-check-input" type="checkbox" id="productIsTaxExempt">
                      <label class="form-check-label" for="productIsTaxExempt">
                          Producto Exento de Impuestos
                      </label>
                  </div>
                  
                  <div class="mt-3">
                      <label class="form-label">Seleccionar Imagen (Opcional)</label>
                      <input class="form-control" type="file" id="productImageFile" accept="image/png, image/jpeg">
                  </div>
                  <div class="text-center mt-2">
                      <img id="imagePreview" src="" style="max-width: 100%; max-height: 150px; display: none; border-radius: 8px;">
                  </div>
`;

html = html.replace(`
                <div class="form-floating">
                    <select class="form-select" id="productCategory" required>
                        <!-- Loaded dynamically -->
                    </select>
                    <label>CategorÃ­a</label>
                </div>
            
                  <div class="form-check mt-3 mb-2">
                      <input class="form-check-input" type="checkbox" id="productIsTaxExempt">
                      <label class="form-check-label" for="productIsTaxExempt">
                          Producto Exento de Impuestos
                      </label>
                  </div>`, formContent);

// Remove the separate upload modal
html = html.replace(/<!-- Upload Image Modal -->[\s\S]*?<\/div>[\s]*<\/div>[\s]*<\/div>[\s]*<\/div>/, '');

fs.writeFileSync('Frontend/pages/products.html', html);
console.log("Updated products.html");
