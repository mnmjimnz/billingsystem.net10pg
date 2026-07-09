const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/products.html', 'utf-8');

const checkboxHTML = `
                  <div class="form-check mt-3 mb-2">
                      <input class="form-check-input" type="checkbox" id="productIsTaxExempt">
                      <label class="form-check-label" for="productIsTaxExempt">
                          Producto Exento de Impuestos
                      </label>
                  </div>`;

if (!html.includes('id="productIsTaxExempt"')) {
    html = html.replace('</form>', checkboxHTML + '\n              </form>');
    
    // Also let's fix the corrupted text
    html = html.replace('CategorA-a', 'Categoría');
    html = html.replace('CA3digo', 'Código');
    
    fs.writeFileSync('Frontend/pages/products.html', html, 'utf-8');
    console.log("Fixed products.html completely");
} else {
    console.log("Already fixed");
}
