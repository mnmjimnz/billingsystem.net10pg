const fs = require('fs');

const htmlContent = `
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Traslados de Sucursal - Sistema de Facturación</title>
    <!-- CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.1/font/bootstrap-icons.css" rel="stylesheet">
    <link href="../assets/css/devextreme-theme.css" rel="stylesheet">
</head>
<body class="bg-light">

    <div class="d-flex h-100 position-relative">
        <!-- Sidebar container -->
        <div id="sidebar-container"></div>

        <main class="main-content">
            <header class="topbar">
                <div class="d-flex align-items-center gap-3">
                    <button class="btn btn-light d-lg-none" id="sidebarToggle"><i class="bi bi-list"></i></button>
                    <h4 class="mb-0 text-dark fw-bold d-none d-sm-block">Traslados de Inventario</h4>
                </div>
                <div class="d-flex align-items-center gap-4">
                    <button class="btn btn-primary shadow-sm rounded-pill px-4 fw-semibold" onclick="openTransferModal()">
                        <i class="bi bi-truck me-2"></i> Nuevo Traslado
                    </button>
                    <!-- User profile container -->
                    <div id="user-profile-container"></div>
                </div>
            </header>

            <div class="content-wrapper p-4">
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
        </main>
    </div>

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

    <!-- Global Toast Container -->
    <div class="toast-container position-fixed bottom-0 end-0 p-3" id="toastContainer"></div>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@microsoft/signalr@7.0.5/dist/browser/signalr.min.js"></script>
    <script src="../assets/js/apiClient.js"></script>
    <script src="stock-transfers.js"></script>
</body>
</html>
`;

const jsContent = `
document.addEventListener('DOMContentLoaded', async () => {
    if (!ApiClient.getToken()) {
        window.location.href = '../index.html';
        return;
    }

    await loadLayout();
    await loadBranches();
    await loadProducts();
    await loadTransfers();
});

let branches = [];
let products = [];
let transferModalInstance;

async function loadLayout() {
    try {
        const sidebarHtml = await fetch('../components/sidebar.html').then(r => r.text());
        document.getElementById('sidebar-container').innerHTML = sidebarHtml;
        const profileHtml = await fetch('../components/user-profile.html').then(r => r.text());
        document.getElementById('user-profile-container').innerHTML = profileHtml;
        document.getElementById('sidebarToggle').addEventListener('click', () => {
            document.querySelector('.sidebar').classList.toggle('show');
        });
        
        const path = window.location.pathname;
        const links = document.querySelectorAll('.sidebar .nav-link');
        links.forEach(link => {
            if (path.includes(link.getAttribute('href'))) {
                link.classList.add('active');
            }
        });
        
        initUserProfile();
    } catch (e) {
        console.error("Error loading layout components", e);
    }
}

async function loadBranches() {
    try {
        branches = await ApiClient.request('/Branches') || [];
        const fromSelect = document.getElementById('fromBranchSelect');
        const toSelect = document.getElementById('toBranchSelect');
        
        const options = branches.map(b => \`<option value="\${b.id}">\${b.name}</option>\`).join('');
        fromSelect.innerHTML = '<option value="">Seleccione origen...</option>' + options;
        toSelect.innerHTML = '<option value="">Seleccione destino...</option>' + options;
    } catch (e) {
        console.error("Error loading branches", e);
    }
}

async function loadProducts() {
    try {
        products = await ApiClient.request('/Products') || [];
        const select = document.getElementById('productSelect');
        const options = products.map(p => \`<option value="\${p.id}">\${p.name}</option>\`).join('');
        select.innerHTML = '<option value="">Seleccione un producto...</option>' + options;
    } catch (e) {
        console.error("Error loading products", e);
    }
}

async function loadTransfers() {
    try {
        const transfers = await ApiClient.request('/StockTransfers') || [];
        const tbody = document.getElementById('transfersList');
        
        if (transfers.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" class="text-center py-4 text-muted">No se han realizado traslados.</td></tr>';
            return;
        }

        tbody.innerHTML = transfers.map(t => \`
            <tr>
                <td>\${new Date(t.createdAt).toLocaleString()}</td>
                <td class="fw-semibold">\${t.productName}</td>
                <td><span class="badge bg-light text-dark border">\${t.fromBranchName}</span></td>
                <td><span class="badge bg-info text-white">\${t.toBranchName}</span></td>
                <td class="fw-bold">\${t.quantity}</td>
                <td>\${t.userName}</td>
                <td class="small text-muted">\${t.notes || ''}</td>
            </tr>
        \`).join('');
    } catch (e) {
        console.error("Error loading transfers", e);
        document.getElementById('transfersList').innerHTML = '<tr><td colspan="7" class="text-center py-4 text-danger">Error al cargar traslados</td></tr>';
    }
}

function openTransferModal() {
    if (!transferModalInstance) {
        transferModalInstance = new bootstrap.Modal(document.getElementById('transferModal'));
    }
    document.getElementById('transferForm').reset();
    document.getElementById('stockAlert').style.display = 'none';
    transferModalInstance.show();
}

async function loadProductStock() {
    const productId = document.getElementById('productSelect').value;
    const branchId = document.getElementById('fromBranchSelect').value;
    const alert = document.getElementById('stockAlert');
    const stockLabel = document.getElementById('availableStock');
    const qtyInput = document.getElementById('transferQuantity');

    if (productId && branchId) {
        try {
            // Need to get specific stock for this branch
            const stock = await ApiClient.request(\`/Products/\${productId}/stock\`);
            const branchStock = stock.find(s => s.branchId == branchId);
            const currentStock = branchStock ? branchStock.stock : 0;
            
            stockLabel.innerText = currentStock;
            alert.style.display = 'block';
            qtyInput.max = currentStock;
        } catch (e) {
            console.error("Error fetching stock", e);
        }
    } else {
        alert.style.display = 'none';
    }
}

async function processTransfer() {
    const productId = document.getElementById('productSelect').value;
    const fromBranchId = document.getElementById('fromBranchSelect').value;
    const toBranchId = document.getElementById('toBranchSelect').value;
    const quantity = document.getElementById('transferQuantity').value;
    const notes = document.getElementById('transferNotes').value;

    if (!productId || !fromBranchId || !toBranchId || !quantity) {
        showToast("Complete todos los campos requeridos.", "error");
        return;
    }

    if (fromBranchId === toBranchId) {
        showToast("La sucursal de origen y destino deben ser diferentes.", "error");
        return;
    }

    const availableStock = parseInt(document.getElementById('availableStock').innerText);
    if (parseInt(quantity) > availableStock) {
        showToast("La cantidad a trasladar supera el stock disponible en la sucursal de origen.", "error");
        return;
    }

    const btn = document.getElementById('btnProcessTransfer');
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Procesando...';

    try {
        await ApiClient.request('/StockTransfers', 'POST', {
            productId: parseInt(productId),
            fromBranchId: parseInt(fromBranchId),
            toBranchId: parseInt(toBranchId),
            quantity: parseInt(quantity),
            notes: notes
        });

        showToast("Traslado procesado exitosamente.", "success");
        transferModalInstance.hide();
        loadTransfers();
    } catch (e) {
        showToast(e.message || "Error al procesar el traslado.", "error");
    } finally {
        btn.disabled = false;
        btn.innerHTML = 'Procesar Traslado';
    }
}

function showToast(message, type = 'info') {
    const toastContainer = document.getElementById('toastContainer');
    const toastId = 'toast' + Date.now();
    const bgColor = type === 'success' ? 'bg-success' : type === 'error' ? 'bg-danger' : 'bg-info';
    
    const toastHtml = \`
        <div id="\${toastId}" class="toast align-items-center text-white \${bgColor} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">\${message}</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    \`;
    toastContainer.insertAdjacentHTML('beforeend', toastHtml);
    const toastElement = document.getElementById(toastId);
    const toast = new bootstrap.Toast(toastElement);
    toast.show();
    toastElement.addEventListener('hidden.bs.toast', () => toastElement.remove());
}
function initUserProfile() {
    document.getElementById('logoutBtn')?.addEventListener('click', logout);
    const userName = localStorage.getItem('userName');
    const roleName = localStorage.getItem('roleName');
    if (userName) {
        document.getElementById('userProfileName').innerText = userName;
        document.getElementById('userProfileRole').innerText = roleName;
    }
}
function logout() {
    ApiClient.clearToken();
    window.location.href = '../index.html';
}
`;

fs.writeFileSync('Frontend/pages/stock-transfers.html', htmlContent, 'utf-8');
fs.writeFileSync('Frontend/pages/stock-transfers.js', jsContent, 'utf-8');
console.log("Created stock-transfers.html and stock-transfers.js");
