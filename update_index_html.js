const fs = require('fs');
let html = fs.readFileSync('Frontend/store/index.html', 'utf8');

// 1. Theme toggle button in navbar
const themeBtn = `
                    <li class="nav-item me-3 d-flex align-items-center">
                        <button class="btn btn-link text-decoration-none p-0" onclick="toggleTheme()" title="Cambiar Tema">
                            <i class="bi bi-moon-stars-fill text-secondary fs-5" id="themeIcon"></i>
                        </button>
                    </li>
`;
// insert before the authMenu li
html = html.replace(
    /<li class="nav-item me-3" id="authMenu">/,
    themeBtn + '\n                    <li class="nav-item me-3" id="authMenu">'
);

// 2. Product Detail Modal
const productModal = `
    <!-- Product Detail Modal -->
    <div class="modal fade" id="productDetailModal" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content" style="border-radius: 20px; overflow: hidden; border: none; box-shadow: var(--store-shadow-hover);">
                <div class="modal-body p-0">
                    <button type="button" class="btn-close position-absolute top-0 end-0 m-3" data-bs-dismiss="modal" style="z-index: 10; background-color: var(--store-card); border-radius: 50%; padding: 10px;"></button>
                    <div class="row g-0">
                        <div class="col-md-6 d-flex align-items-center justify-content-center" style="background-color: #fff; min-height: 300px;">
                            <img src="" id="modalProductImg" class="img-fluid" style="max-height: 400px; object-fit: contain; padding: 20px;" alt="">
                        </div>
                        <div class="col-md-6 p-4 p-md-5 d-flex flex-column">
                            <h3 id="modalProductTitle" class="fw-bold mb-3" style="color: var(--store-text);"></h3>
                            <h4 id="modalProductPrice" class="fw-bold mb-4" style="color: var(--store-text);"></h4>
                            <p id="modalProductDesc" class="text-muted mb-4 flex-grow-1" style="line-height: 1.6;"></p>
                            <button class="btn-minimal btn-lg w-100 mt-auto" id="modalAddToCartBtn">
                                <i class="bi bi-cart-plus me-2"></i>Agregar al Carrito
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
`;
// insert before the scripts
html = html.replace(
    /<script src="https:\/\/cdn\.jsdelivr\.net\/npm\/bootstrap@5\.3\.3\/dist\/js\/bootstrap\.bundle\.min\.js"><\/script>/,
    productModal + '\n    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>'
);

fs.writeFileSync('Frontend/store/index.html', html);
console.log("index.html updated");
