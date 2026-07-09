const fs = require('fs');

let html = fs.readFileSync('Frontend/pages/purchases.html', 'utf-8');

// Replace the form wrapper
const old_wrapper = /<div class="card-body p-3 border-bottom bg-light bg-opacity-50">\s*<div class="form-floating mb-3 position-relative">/;
const new_wrapper = `<div class="card-body p-3 border-bottom bg-light bg-opacity-50 d-none d-lg-block" id="desktop-checkout-parent">
                                <div id="checkout-form-container">
                                  <div class="form-floating mb-3 position-relative">`;
html = html.replace(old_wrapper, new_wrapper);

// Close the new div wrapper (checkout-form-container) just before the next card-body
const old_end = /(<div class="card-body p-0" style="max-height: calc\(100vh - 480px\); overflow-y: auto;">)/;
const new_end = `</div>\n                              </div>\n                              $1`;
html = html.replace(old_end, new_end);

// Add the modal at the end of body
const modal_html = `
    <!-- Mobile Checkout Modal -->
    <div class="modal fade" id="mobileCheckoutModal" tabindex="-1">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content border-0">
          <div class="modal-header border-0 pb-0">
            <h5 class="modal-title fw-bold">Completar Compra</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <div class="modal-body p-4" id="mobile-checkout-modal-body">
            <!-- Form moves here on mobile -->
          </div>
          <div class="modal-footer border-0 pt-0">
            <button class="btn btn-primary w-100 py-3 fs-5 shadow-sm fw-bold rounded-pill" id="btn-modal-save-purchase">
                <i class="bi bi-check-circle me-2"></i> Confirmar Ingreso
            </button>
          </div>
        </div>
      </div>
    </div>
`;
html = html.replace('</body>', modal_html + '\n</body>');

fs.writeFileSync('Frontend/pages/purchases.html', html, 'utf-8');

let js = fs.readFileSync('Frontend/pages/purchases.js', 'utf-8');

// Fix button click logic
const old_click = /document\.getElementById\('btn-save-purchase'\)\.addEventListener\('click', savePurchase\);/;
const new_click = `
    document.getElementById('btn-save-purchase').addEventListener('click', () => {
        if (window.innerWidth < 992) {
            const m = new bootstrap.Modal(document.getElementById('mobileCheckoutModal'));
            m.show();
        } else {
            savePurchase();
        }
    });
    
    document.getElementById('btn-modal-save-purchase').addEventListener('click', () => {
        savePurchase();
    });
    
    function handleResponsiveCheckout() {
        const formContainer = document.getElementById('checkout-form-container');
        const desktopParent = document.getElementById('desktop-checkout-parent');
        const mobileParent = document.getElementById('mobile-checkout-modal-body');

        if (window.innerWidth < 992) {
            if (formContainer && formContainer.parentElement !== mobileParent) {
                mobileParent.appendChild(formContainer);
            }
        } else {
            if (formContainer && formContainer.parentElement !== desktopParent) {
                desktopParent.appendChild(formContainer);
            }
        }
    }
    
    window.addEventListener('resize', handleResponsiveCheckout);
    handleResponsiveCheckout();
`;
js = js.replace(old_click, new_click);

// Also fix the spinner on modal button
js = js.replace("const btn = document.getElementById('btn-save-purchase');", 
                "const btn = document.getElementById('btn-save-purchase');\n        const btnModal = document.getElementById('btn-modal-save-purchase');");

js = js.replace("btn.disabled = true;", "btn.disabled = true; if(btnModal) btnModal.disabled = true;");
js = js.replace("btn.innerHTML = '<span class=\"spinner-border spinner-border-sm me-2\"></span> Procesando...';",
                "btn.innerHTML = '<span class=\"spinner-border spinner-border-sm me-2\"></span> Procesando...'; if(btnModal) btnModal.innerHTML = '<span class=\"spinner-border spinner-border-sm me-2\"></span> Procesando...';");

js = js.replace("document.getElementById('btn-save-purchase').disabled = false;",
                "document.getElementById('btn-save-purchase').disabled = false; if(document.getElementById('btn-modal-save-purchase')) document.getElementById('btn-modal-save-purchase').disabled = false;");

js = js.replace("document.getElementById('btn-save-purchase').innerHTML = '<i class=\"bi bi-check-circle me-2\"></i> Procesar Ingreso';",
                "document.getElementById('btn-save-purchase').innerHTML = '<i class=\"bi bi-check-circle me-2\"></i> Procesar Ingreso'; if(document.getElementById('btn-modal-save-purchase')) document.getElementById('btn-modal-save-purchase').innerHTML = '<i class=\"bi bi-check-circle me-2\"></i> Confirmar Ingreso';");


// Hide modal on success
js = js.replace("cart = [];", 
                "cart = [];\n        const modalEl = document.getElementById('mobileCheckoutModal'); if(modalEl) { const modal = bootstrap.Modal.getInstance(modalEl); if(modal) modal.hide(); }");

fs.writeFileSync('Frontend/pages/purchases.js', js, 'utf-8');
