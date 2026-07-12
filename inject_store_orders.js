const fs = require('fs');

// 1. IOrderRepository.cs
let irepo = fs.readFileSync('Backend/BillingSystem.Domain/Interfaces/IOrderRepository.cs', 'utf8');
if (!irepo.includes('GetByCustomerIdAsync')) {
    irepo = irepo.replace(
        /Task<IEnumerable<Order>> GetAllAsync\(\);/,
        'Task<IEnumerable<Order>> GetAllAsync();\n    Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId);'
    );
    fs.writeFileSync('Backend/BillingSystem.Domain/Interfaces/IOrderRepository.cs', irepo);
}

// 2. OrderRepository.cs
let repo = fs.readFileSync('Backend/BillingSystem.Infrastructure/Repositories/OrderRepository.cs', 'utf8');
if (!repo.includes('GetByCustomerIdAsync')) {
    const methodStr = `
    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId)
    {
        using var connection = _db.CreateConnection();
        var sql = @"
            SELECT * FROM orders WHERE CustomerId = @CustomerId ORDER BY CreatedAt DESC;
        ";
        var orders = await connection.QueryAsync<Order>(sql, new { CustomerId = customerId });
        
        foreach(var order in orders)
        {
            var detailSql = @"
                SELECT od.*, p.Name as ProductName 
                FROM orderdetails od
                JOIN products p ON od.ProductId = p.Id
                WHERE od.OrderId = @OrderId;
            ";
            order.Details = (await connection.QueryAsync<OrderDetail>(detailSql, new { OrderId = order.Id })).ToList();
        }
        return orders;
    }
`;
    repo = repo.replace(
        /public async Task<int> AddOrderAsync/,
        methodStr + '\n    public async Task<int> AddOrderAsync'
    );
    fs.writeFileSync('Backend/BillingSystem.Infrastructure/Repositories/OrderRepository.cs', repo);
}

// 3. StoreController.cs
let ctrl = fs.readFileSync('Backend/BillingSystem.Api/Controllers/StoreController.cs', 'utf8');
if (!ctrl.includes('GetMyOrders')) {
    const endpointStr = `
    [HttpGet("orders")]
    [Authorize]
    public async Task<IActionResult> GetMyOrders()
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        var customer = await _customerRepository.GetByEmailAsync(email);
        if (customer == null) return Unauthorized();

        var orders = await _orderRepository.GetByCustomerIdAsync(customer.Id);
        return Ok(orders);
    }
`;
    ctrl = ctrl.replace(
        /return Ok\(new \{ message = "Pedido realizado con éxito", orderId = orderId \}\);\s*\}/,
        'return Ok(new { message = "Pedido realizado con éxito", orderId = orderId });\n    }\n' + endpointStr
    );
    fs.writeFileSync('Backend/BillingSystem.Api/Controllers/StoreController.cs', ctrl);
}

// 4. index.html (Add My Orders link in Navbar and Modal)
const pages = ['Frontend/store/index.html', 'Frontend/store/cart.html'];
const myOrdersLink = `<a href="#" class="text-decoration-none me-3" onclick="showMyOrdersModal()"><i class="bi bi-box-seam"></i> Mis Pedidos</a>`;
const modalHtml = `
    <!-- My Orders Modal -->
    <div class="modal fade" id="myOrdersModal" tabindex="-1">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header border-secondary">
                    <h5 class="modal-title"><i class="bi bi-box-seam"></i> Mis Pedidos</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body" id="myOrdersContent">
                    <div class="text-center text-muted py-4">Cargando pedidos...</div>
                </div>
            </div>
        </div>
    </div>
`;

pages.forEach(page => {
    let html = fs.readFileSync(page, 'utf8');
    if (!html.includes('showMyOrdersModal()')) {
        // add to navbar after cart icon or before logout
        html = html.replace(
            /<a href="#" class="text-danger text-decoration-none" onclick="logout\(\)">/,
            myOrdersLink + '\n                        <a href="#" class="text-danger text-decoration-none" onclick="logout()">'
        );
        // add modal before closing body
        html = html.replace(
            /<\/body>/,
            modalHtml + '\n</body>'
        );
        
        // bump cache
        const ts = Date.now();
        html = html.replace(/app\.js\?v=[0-9]+/g, `app.js?v=${ts}`);
        fs.writeFileSync(page, html);
    }
});

// 5. app.js
let appJs = fs.readFileSync('Frontend/store/app.js', 'utf8');
if (!appJs.includes('function showMyOrdersModal')) {
    const jsLogic = `
window.showMyOrdersModal = async function() {
    const token = localStorage.getItem('storeToken');
    if (!token) {
        Swal.fire('Atención', 'Debes iniciar sesión para ver tus pedidos', 'warning');
        return;
    }

    const modalEl = document.getElementById('myOrdersModal');
    if(!modalEl) return;
    const modal = new bootstrap.Modal(modalEl);
    modal.show();

    const content = document.getElementById('myOrdersContent');
    content.innerHTML = '<div class="text-center text-muted py-4"><div class="spinner-border text-primary" role="status"></div><br>Cargando...</div>';

    try {
        const res = await fetch(\`\${API_URL}/Store/orders\`, {
            headers: { 'Authorization': \`Bearer \${token}\` }
        });

        if (res.ok) {
            const orders = await res.json();
            if (orders.length === 0) {
                content.innerHTML = '<div class="text-center text-muted py-4">No has realizado ningún pedido aún.</div>';
                return;
            }

            let html = '<div class="accordion" id="ordersAccordion">';
            orders.forEach((o, index) => {
                const date = new Date(o.createdAt).toLocaleString();
                let statusBadge = '';
                switch(o.status) {
                    case 'PENDING': statusBadge = '<span class="badge bg-warning text-dark">Pendiente</span>'; break;
                    case 'CONFIRMED': statusBadge = '<span class="badge bg-info text-dark">Confirmado</span>'; break;
                    case 'SHIPPED': statusBadge = '<span class="badge bg-primary">Enviado</span>'; break;
                    case 'DELIVERED': statusBadge = '<span class="badge bg-success">Entregado</span>'; break;
                    case 'CANCELLED': statusBadge = '<span class="badge bg-danger">Cancelado</span>'; break;
                    default: statusBadge = \`<span class="badge bg-secondary">\${o.status}</span>\`; break;
                }

                let itemsHtml = '<ul class="list-group mb-3">';
                o.details.forEach(d => {
                    itemsHtml += \`<li class="list-group-item d-flex justify-content-between align-items-center">
                        <div>\${d.productName || 'Producto ID: ' + d.productId} <span class="text-muted">x\${d.quantity}</span></div>
                        <span>$\${d.total.toFixed(2)}</span>
                    </li>\`;
                });
                itemsHtml += '</ul>';

                html += \`
                    <div class="accordion-item mb-2 border">
                        <h2 class="accordion-header" id="heading\${o.id}">
                            <button class="accordion-button \${index === 0 ? '' : 'collapsed'}" type="button" data-bs-toggle="collapse" data-bs-target="#collapse\${o.id}">
                                <div class="d-flex justify-content-between w-100 pe-3">
                                    <strong>\${o.orderNumber || 'Pedido #'+o.id}</strong>
                                    <span>\${date}</span>
                                    \${statusBadge}
                                </div>
                            </button>
                        </h2>
                        <div id="collapse\${o.id}" class="accordion-collapse collapse \${index === 0 ? 'show' : ''}" data-bs-parent="#ordersAccordion">
                            <div class="accordion-body">
                                \${itemsHtml}
                                <div class="d-flex justify-content-between mt-2">
                                    <span>Forma de pago: <strong>\${o.paymentMethod || 'EFECTIVO'}</strong></span>
                                    <h5 class="mb-0">Total: <strong>$\${o.total.toFixed(2)}</strong></h5>
                                </div>
                                <div class="mt-2 text-muted small">
                                    <strong>Dirección:</strong> \${o.deliveryAddress || 'N/A'}<br>
                                    \${o.notes ? \`<strong>Notas:</strong> \${o.notes}\` : ''}
                                </div>
                            </div>
                        </div>
                    </div>
                \`;
            });
            html += '</div>';
            content.innerHTML = html;
        } else {
            content.innerHTML = '<div class="text-center text-danger py-4">Error al cargar los pedidos.</div>';
        }
    } catch (e) {
        content.innerHTML = '<div class="text-center text-danger py-4">Error de conexión.</div>';
    }
};
`;
    appJs += '\n' + jsLogic;
    fs.writeFileSync('Frontend/store/app.js', appJs);
}

console.log("Injected orders feature.");
