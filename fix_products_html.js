const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/products.html', 'utf-8');

const modals = `

    <!-- Stock Breakdown Modal -->
    <div class="modal fade" id="stockBreakdownModal" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content border-0">
                <div class="modal-header border-0 pb-0">
                    <h5 class="modal-title fw-bold">Stock por Sucursal</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body p-4">
                    <h6 class="text-secondary mb-3" id="stockBreakdownProductName"></h6>
                    <table class="table table-sm table-borderless">
                        <tbody id="stockBreakdownBody">
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>

    <!-- Barcodes Modal -->
    <div class="modal fade" id="barcodesModal" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content border-0">
                <div class="modal-header border-0 pb-0">
                    <h5 class="modal-title fw-bold">Códigos de Barras</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body p-4">
                    <h6 class="text-secondary mb-3" id="barcodesProductName"></h6>
                    <div id="barcodesList" class="d-flex flex-wrap gap-2"></div>
                </div>
            </div>
        </div>
    </div>
</body>`;

html = html.replace('</body>', modals);
fs.writeFileSync('Frontend/pages/products.html', html, 'utf-8');
console.log("Added modals to products.html");
