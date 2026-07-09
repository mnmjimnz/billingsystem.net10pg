const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/products.html', 'utf8');

// 1. Add column header
html = html.replace('<th>CÃ³digo</th>', '<th>Imagen</th>\n                                        <th>CÃ³digo</th>');

// 2. Add Upload Image Modal before closing body
const uploadModal = `
    <!-- Upload Image Modal -->
    <div class="modal fade" id="uploadImageModal" tabindex="-1">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content border-0">
          <div class="modal-header border-0 pb-0">
            <h5 class="modal-title fw-bold">Subir Imagen</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <div class="modal-body p-4">
            <input type="hidden" id="uploadImageProductId">
            <div class="mb-3">
                <label class="form-label">Selecciona una imagen (PNG, JPG)</label>
                <input class="form-control" type="file" id="productImageFile" accept="image/png, image/jpeg">
            </div>
            <div class="text-center mt-3">
                <img id="imagePreview" src="" style="max-width: 100%; max-height: 200px; display: none; border-radius: 8px;">
            </div>
          </div>
          <div class="modal-footer border-0 pt-0 pe-4 pb-4">
            <button type="button" class="btn btn-light" data-bs-dismiss="modal">Cancelar</button>
            <button type="button" class="btn btn-primary px-4" onclick="uploadProductImage()">Subir</button>
          </div>
        </div>
      </div>
    </div>
</body>`;

html = html.replace('</body>', uploadModal);

fs.writeFileSync('Frontend/pages/products.html', html);
console.log("Updated products.html");
