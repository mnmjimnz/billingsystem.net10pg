const fs = require('fs');
const purchasesHtml = fs.readFileSync('Frontend/pages/purchases.html', 'utf-8');

const title = 'Traslados de Sucursal - Sistema de Facturación Premium';
const jsFile = 'stock-transfers.js';

const pageContent = `
            <div class="page-content p-4">
                <h2 class="mb-4 fw-bold text-dark">Traslados de Sucursal</h2>
                <div class="d-flex justify-content-between align-items-center mb-4">
                    <h4 class="fw-bold mb-0 text-secondary">Historial de Traslados</h4>
                    <button class="btn btn-primary shadow-sm rounded-pill px-4 fw-semibold" onclick="openTransferModal()">
                        <i class="bi bi-truck me-2"></i> Nuevo Traslado
                    </button>
                </div>
                <div class="card shadow-sm border-0 rounded-4">
                    <div class="card-header bg-white border-bottom pt-4 pb-3 px-4">
                        <div class="row align-items-center">
                            <div class="col-md-6">
                                <h5 class="fw-bold mb-0 text-secondary"><i class="bi bi-clock-history me-2"></i> Historial de Traslados</h5>
                            </div>
                        </div>
                    </div>
                    <div class="card-body p-0">
                        <div class="table-responsive">
                            <table class="table table-hover align-middle mb-0" id="transfersTable">
                                <thead class="table-light">
                                    <tr>
                                        <th class="ps-4">Fecha</th>
                                        <th>Producto</th>
                                        <th>Origen</th>
                                        <th>Destino</th>
                                        <th>Cantidad</th>
                                        <th>Usuario</th>
                                        <th>Notas</th>
                                    </tr>
                                </thead>
                                <tbody id="transfersList">
                                    <tr><td colspan="7" class="text-center py-4">Cargando traslados...</td></tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
`;

const modalContent = `
    <!-- Transfer Modal -->
    <div class="modal fade" id="transferModal" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content border-0 shadow-lg rounded-4">
                <div class="modal-header border-bottom-0 pb-0 px-4 pt-4">
                    <h5 class="modal-title fw-bold" id="modalTitle">Nuevo Traslado de Inventario</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body p-4">
                    <form id="transferForm" onsubmit="event.preventDefault();">
                        <div class="form-floating mb-3">
                            <select class="form-select" id="productSelect" onchange="loadProductStock()" required>
                                <option value="">Seleccione un producto...</option>
                            </select>
                            <label>Producto a Trasladar</label>
                        </div>
                        <div class="row g-3 mb-3">
                            <div class="col-md-6">
                                <div class="form-floating">
                                    <select class="form-select" id="fromBranchSelect" onchange="loadProductStock()" required>
                                        <option value="">Seleccione origen...</option>
                                    </select>
                                    <label>Sucursal de Origen</label>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-floating">
                                    <select class="form-select" id="toBranchSelect" required>
                                        <option value="">Seleccione destino...</option>
                                    </select>
                                    <label>Sucursal de Destino</label>
                                </div>
                            </div>
                        </div>
                        <div class="alert alert-info py-2" id="stockAlert" style="display:none;">
                            Stock disponible en Origen: <strong id="availableStock">0</strong>
                        </div>
                        <div class="form-floating mb-3">
                            <input type="number" class="form-control" id="transferQuantity" min="1" required>
                            <label>Cantidad a Trasladar</label>
                        </div>
                        <div class="form-floating mb-3">
                            <textarea class="form-control" id="transferNotes" style="height: 100px"></textarea>
                            <label>Notas (Opcional)</label>
                        </div>
                    </form>
                </div>
                <div class="modal-footer border-top-0 px-4 pb-4">
                    <button type="button" class="btn btn-light" data-bs-dismiss="modal">Cancelar</button>
                    <button type="button" class="btn btn-primary px-4" onclick="processTransfer()" id="btnProcessTransfer">Procesar Traslado</button>
                </div>
            </div>
        </div>
    </div>
`;

// Extract layout from purchases.html
const pStart = purchasesHtml.indexOf('<div class="page-content');
const pEnd = purchasesHtml.indexOf('</main>');
let cleanHtml = purchasesHtml.substring(0, pStart) + pageContent + '\n        ' + purchasesHtml.substring(pEnd);

// Replace Modals
const mStart = cleanHtml.indexOf('<!-- Purchase Modal -->');
const mEnd = cleanHtml.indexOf('<!-- Global Toast Container -->');
cleanHtml = cleanHtml.substring(0, mStart) + modalContent + '\n    ' + cleanHtml.substring(mEnd);

// Replace Title
cleanHtml = cleanHtml.replace(/<title>.*?<\/title>/, `<title>${title}</title>`);

// Replace Scripts
cleanHtml = cleanHtml.replace(/<script src="purchases\.js.*?"><\/script>/, `<script src="${jsFile}"></script>`);

fs.writeFileSync('Frontend/pages/stock-transfers.html', cleanHtml, 'utf-8');
console.log('Successfully reconstructed stock-transfers.html');
