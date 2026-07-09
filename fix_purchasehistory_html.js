const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/purchase-history.html', 'utf-8');

const modalHTML = `
    <!-- Purchase Details Modal -->
    <div class="modal fade" id="purchaseDetailsModal" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-scrollable">
            <div class="modal-content border-0 shadow">
                <div class="modal-header border-bottom-0 pb-0">
                    <h5 class="modal-title fw-bold">Detalles de la Compra</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body">
                    <div class="row mb-4">
                        <div class="col-md-4">
                            <p class="text-secondary small mb-1">No. Factura / Doc.</p>
                            <h6 class="fw-bold" id="detailInvoice">-</h6>
                        </div>
                        <div class="col-md-4">
                            <p class="text-secondary small mb-1">Fecha</p>
                            <h6 class="fw-bold" id="detailDate">-</h6>
                        </div>
                        <div class="col-md-4">
                            <p class="text-secondary small mb-1">Proveedor</p>
                            <h6 class="fw-bold" id="detailSupplier">-</h6>
                        </div>
                        <div class="col-md-6 mt-3">
                            <p class="text-secondary small mb-1">Estado / Tipo de Pago</p>
                            <h6 class="fw-bold" id="detailStatus">-</h6>
                        </div>
                        <div class="col-md-6 mt-3">
                            <p class="text-secondary small mb-1">Total</p>
                            <h6 class="fw-bold text-success fs-5" id="detailTotal">-</h6>
                        </div>
                    </div>
                    
                    <h6 class="fw-bold mb-3"><i class="bi bi-box-seam me-2"></i>Productos Adquiridos</h6>
                    <div class="table-responsive">
                        <table class="table table-hover align-middle">
                            <thead class="table-light">
                                <tr>
                                    <th>C&oacute;digo</th>
                                    <th>Producto</th>
                                    <th class="text-center">Cant.</th>
                                    <th class="text-end">Costo Unit.</th>
                                    <th class="text-end">Subtotal</th>
                                </tr>
                            </thead>
                            <tbody id="detailProductsBody">
                                <!-- Rendered by JS -->
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer border-top-0 pt-0">
                    <button type="button" class="btn btn-light" data-bs-dismiss="modal">Cerrar</button>
                </div>
            </div>
        </div>
    </div>
`;

if (!html.includes('purchaseDetailsModal')) {
    html = html.replace('<!-- Libraries -->', modalHTML + '\n    <!-- Libraries -->');
    fs.writeFileSync('Frontend/pages/purchase-history.html', html, 'utf-8');
    console.log("Modal added to purchase-history.html");
} else {
    console.log("Modal already exists");
}
