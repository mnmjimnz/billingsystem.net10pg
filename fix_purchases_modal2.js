const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/purchases.html', 'utf-8');

// 1. Delete desktop-checkout-parent entirely.
html = html.replace(/<div class="card-body p-3 border-bottom bg-light bg-opacity-50 d-none d-lg-block" id="desktop-checkout-parent">[\s\S]*?<\/div>\s*<\/div>/, '');

// 2. Put checkout-form-container inside the mobile-checkout-modal-body, replacing its contents
const formHtml = `
            <div id="checkout-form-container">
                <div class="form-floating mb-3 position-relative">
                <select class="form-select border-info" id="branchSelect" onchange="updateAvailableFunds()">
                </select>
                <label>Sucursal</label>
                <span class="badge bg-success position-absolute top-0 end-0 mt-2 me-2" id="available-funds-badge">Fondos: $0.00</span>
            </div>
            <div class="form-floating mb-3">
                <select class="form-select border-info" id="supplierSelect">
                </select>
                <label>Proveedor</label>
            </div>
            <div class="form-floating mb-3">
                <input type="text" class="form-control border-info" id="invoiceInput" placeholder="Factura">
                <label>Nº de Factura/Documento</label>
            </div>
            <div class="form-floating mb-3">
                <select class="form-select border-info" id="paymentTypeSelect" onchange="toggleAdvancePayment()">
                    <option value="CASH">Efectivo (Contado)</option>
                    <option value="CREDIT">Crédito</option>
                </select>
                <label>Tipo de Pago</label>
            </div>
            <div class="form-floating mb-3" id="divAdvancePayment" style="display:none;">
                <input type="number" class="form-control border-info" id="advanceInput" step="0.01" min="0" value="0">
                <label>Abono Inicial</label>
            </div>
            </div>
`;

html = html.replace('<!-- Form moves here on mobile -->', formHtml);

// 3. The sidebar "btn-save-purchase" should open the modal, not save directly!
html = html.replace(
    '<button class="btn btn-primary text-white w-100 py-3 fs-5 shadow-sm fw-bold rounded-pill" id="btn-save-purchase">',
    '<button class="btn btn-primary text-white w-100 py-3 fs-5 shadow-sm fw-bold rounded-pill" data-bs-toggle="modal" data-bs-target="#mobileCheckoutModal">'
);

fs.writeFileSync('Frontend/pages/purchases.html', html, 'utf-8');
console.log("Updated purchases.html layout");
