const fs = require('fs');
let html = fs.readFileSync('Frontend/pages/reports.html', 'utf-8');

const replacement = `<div class="page-content p-4">
                <h2 class="mb-4 fw-bold text-dark"><i class="bi bi-briefcase-fill text-primary me-2"></i> Módulo Contable y Reportes Empresariales</h2>
                
                <!-- Filtros Universales -->
                <div class="card shadow-sm border-0 mb-4 rounded-3">
                    <div class="card-body bg-white rounded-3">
                        <div class="d-flex justify-content-between align-items-center mb-3">`;

html = html.replace(/<div class="page-content p-4">[\s\S]*?<h6 class="fw-bold mb-0 text-secondary text-uppercase"/, replacement + `\n                            <h6 class="fw-bold mb-0 text-secondary text-uppercase"`);

fs.writeFileSync('Frontend/pages/reports.html', html, 'utf-8');
