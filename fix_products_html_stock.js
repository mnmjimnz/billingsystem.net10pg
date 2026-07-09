const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/products.html', 'utf-8');

const modalHtml = `
    <!-- Stock Breakdown Modal -->
    <div class="modal fade" id="stockBreakdownModal" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content border-0 shadow-lg rounded-4">
                <div class="modal-header border-bottom-0 pb-0 px-4 pt-4">
                    <h5 class="modal-title fw-bold">Stock por Sucursal</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body p-4">
                    <h6 class="text-primary mb-3" id="stockBreakdownProductName"></h6>
                    <table class="table table-sm table-bordered">
                        <thead class="table-light">
                            <tr>
                                <th>Sucursal</th>
                                <th class="text-end">Stock Disponible</th>
                            </tr>
                        </thead>
                        <tbody id="stockBreakdownBody">
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
`;

if (!html.includes('id="stockBreakdownModal"')) {
    html = html.replace('<!-- Global Toast Container -->', modalHtml + '\n    <!-- Global Toast Container -->');
    fs.writeFileSync('Frontend/pages/products.html', html, 'utf-8');
    console.log("Added stockBreakdownModal to products.html");
}
