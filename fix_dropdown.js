const fs = require('fs');

const targetMenu = `<li class="nav-item me-3" id="userMenu" style="display: none;">
                        <span class="me-2">Hola, <b id="customerName"></b></span>
                        <a href="#" class="text-decoration-none me-3" onclick="showMyOrdersModal()"><i class="bi bi-box-seam"></i> Mis Pedidos</a>
                        <a href="#" class="text-danger text-decoration-none" onclick="logout()"><i class="bi bi-box-arrow-right"></i> Salir</a>
                    </li>`;

// The regex needs to be a bit flexible because of indentation
const targetRegex = /<li class="nav-item me-3" id="userMenu" style="display: none;">\s*<span class="me-2">Hola, <b id="customerName"><\/b><\/span>\s*<a href="#" class="text-decoration-none me-3" onclick="showMyOrdersModal\(\)"><i class="bi bi-box-seam"><\/i> Mis Pedidos<\/a>\s*<a href="#" class="text-danger text-decoration-none" onclick="logout\(\)"><i class="bi bi-box-arrow-right"><\/i> Salir<\/a>\s*<\/li>/;

const replacementMenu = `<li class="nav-item dropdown me-3" id="userMenu" style="display: none;">
                        <a class="nav-link dropdown-toggle p-0" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                            Hola, <b id="customerName"></b>
                        </a>
                        <ul class="dropdown-menu dropdown-menu-end shadow border-0 mt-2">
                            <li><a class="dropdown-item py-2" href="#" onclick="showMyOrdersModal()"><i class="bi bi-box-seam me-2 text-primary"></i> Mis Pedidos</a></li>
                            <li><hr class="dropdown-divider"></li>
                            <li><a class="dropdown-item text-danger py-2" href="#" onclick="logout()"><i class="bi bi-box-arrow-right me-2"></i> Salir</a></li>
                        </ul>
                    </li>`;

['Frontend/store/index.html', 'Frontend/store/cart.html'].forEach(file => {
    let content = fs.readFileSync(file, 'utf8');
    if (targetRegex.test(content)) {
        content = content.replace(targetRegex, replacementMenu);
        fs.writeFileSync(file, content);
        console.log("Updated", file);
    } else {
        console.log("Not found in", file);
    }
});
