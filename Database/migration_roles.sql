-- Migration to add Permissions and RolePermissions

CREATE TABLE IF NOT EXISTS Permissions (
    Id SERIAL PRIMARY KEY,
    SystemName VARCHAR(100) NOT NULL UNIQUE,
    DisplayName VARCHAR(100) NOT NULL,
    Description VARCHAR(255),
    Module VARCHAR(50) NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS RolePermissions (
    RoleId INT REFERENCES Roles(Id) ON DELETE CASCADE,
    PermissionId INT REFERENCES Permissions(Id) ON DELETE CASCADE,
    PRIMARY KEY (RoleId, PermissionId)
);

-- Insert Default Permissions
INSERT INTO Permissions (SystemName, DisplayName, Module, Description) VALUES 
('VIEW_DASHBOARD', 'Ver Dashboard', 'Dashboard', 'Permite acceder al panel principal'),
('MANAGE_POS', 'Acceso a POS', 'Ventas', 'Permite realizar ventas en el Punto de Venta'),
('MANAGE_SALES', 'Ver Ventas', 'Ventas', 'Permite ver el historial de ventas'),
('MANAGE_PURCHASES', 'Gestionar Compras', 'Compras', 'Permite registrar y ver compras'),
('MANAGE_RECEIVABLES', 'Cuentas por Cobrar', 'Cobros', 'Permite gestionar créditos y abonos'),
('MANAGE_PRODUCTS', 'Gestionar Productos', 'Inventario', 'Permite crear, editar y ver productos'),
('VIEW_KARDEX', 'Ver Kardex', 'Inventario', 'Permite ver los movimientos de inventario'),
('MANAGE_CATEGORIES', 'Gestionar Categorías', 'Inventario', 'Permite crear y editar categorías'),
('MANAGE_CUSTOMERS', 'Gestionar Clientes', 'Contactos', 'Permite registrar y editar clientes'),
('MANAGE_SUPPLIERS', 'Gestionar Proveedores', 'Contactos', 'Permite registrar y editar proveedores'),
('MANAGE_USERS', 'Gestionar Usuarios', 'Configuración', 'Permite crear y editar usuarios del sistema'),
('MANAGE_ROLES', 'Gestionar Roles', 'Configuración', 'Permite crear roles y asignar permisos')
ON CONFLICT (SystemName) DO NOTHING;

-- Assign all permissions to Admin Role (assuming Admin is RoleId = 1)
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT 1, Id FROM Permissions
ON CONFLICT DO NOTHING;
