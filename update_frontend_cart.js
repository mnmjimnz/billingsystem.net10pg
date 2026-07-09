const fs = require('fs');

let html = fs.readFileSync('Frontend/store/cart.html', 'utf8');

const paymentHtml = `
                                <div class="mb-4">
                                    <label class="form-label text-muted small fw-bold text-uppercase">Forma de Pago</label>
                                    <div class="btn-group w-100" role="group" id="paymentMethodGroup">
                                        <input type="radio" class="btn-check" name="paymentMethod" id="payEfectivo" value="EFECTIVO" autocomplete="off" checked>
                                        <label class="btn btn-outline-secondary" for="payEfectivo">Efectivo</label>
                                      
                                        <input type="radio" class="btn-check" name="paymentMethod" id="payTarjeta" value="TARJETA" autocomplete="off">
                                        <label class="btn btn-outline-secondary" for="payTarjeta">Tarjeta</label>
                                      
                                        <input type="radio" class="btn-check" name="paymentMethod" id="payTransferencia" value="TRANSFERENCIA" autocomplete="off">
                                        <label class="btn btn-outline-secondary" for="payTransferencia">Transferencia</label>
                                    </div>
                                </div>
`;

if (!html.includes('paymentMethodGroup')) {
    html = html.replace(
        /<div class="mb-4">\s*<label class="form-label text-muted small fw-bold text-uppercase">Notas Adicionales<\/label>/,
        paymentHtml + '\n                                <div class="mb-4">\n                                    <label class="form-label text-muted small fw-bold text-uppercase">Notas Adicionales</label>'
    );
    
    // bump cache again
    const ts = Date.now();
    html = html.replace(/app\.js\?v=[0-9]+/g, `app.js?v=${ts}`);
    fs.writeFileSync('Frontend/store/cart.html', html);
    console.log("cart.html updated");
}

let appJs = fs.readFileSync('Frontend/store/app.js', 'utf8');
if (!appJs.includes('document.querySelector(\'input[name="paymentMethod"]:checked\')')) {
    appJs = appJs.replace(
        /const notes = document\.getElementById\('checkoutNotes'\)\.value;/,
        `const notes = document.getElementById('checkoutNotes').value;
    const paymentMethodEl = document.querySelector('input[name="paymentMethod"]:checked');
    const paymentMethod = paymentMethodEl ? paymentMethodEl.value : 'EFECTIVO';`
    );
    
    appJs = appJs.replace(
        /Notes: notes/,
        'Notes: notes,\n            PaymentMethod: paymentMethod'
    );
    fs.writeFileSync('Frontend/store/app.js', appJs);
    console.log("app.js updated");
}
