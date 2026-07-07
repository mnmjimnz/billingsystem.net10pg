-- Migration for Orders Module

-- 1. Add Coordinates to Branches
ALTER TABLE Branches ADD COLUMN IF NOT EXISTS Latitude DECIMAL(10, 8);
ALTER TABLE Branches ADD COLUMN IF NOT EXISTS Longitude DECIMAL(11, 8);

-- 2. Create Orders Table
CREATE TABLE IF NOT EXISTS Orders (
    Id SERIAL PRIMARY KEY,
    OrderNumber VARCHAR(50) NOT NULL UNIQUE,
    Date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CustomerId INT REFERENCES Customers(Id),
    BranchId INT REFERENCES Branches(Id),
    Status VARCHAR(50) NOT NULL DEFAULT 'PENDING', -- PENDING, IN_TRANSIT, DELIVERED, CANCELLED
    DeliveryAddress VARCHAR(255) NOT NULL,
    Latitude DECIMAL(10, 8) NOT NULL,
    Longitude DECIMAL(11, 8) NOT NULL,
    ReceiverName VARCHAR(100),
    DeliveredAt TIMESTAMP,
    Notes TEXT,
    Total DECIMAL(12,2) NOT NULL DEFAULT 0,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

-- 3. Create OrderDetails Table
CREATE TABLE IF NOT EXISTS OrderDetails (
    Id SERIAL PRIMARY KEY,
    OrderId INT REFERENCES Orders(Id) ON DELETE CASCADE,
    ProductId INT REFERENCES Products(Id),
    Quantity INT NOT NULL,
    Price DECIMAL(12,2) NOT NULL,
    Total DECIMAL(12,2) NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

-- 4. Add Permission
INSERT INTO Permissions (SystemName, DisplayName, Module, Description) 
VALUES ('MANAGE_ORDERS', 'Gestionar Pedidos', 'Ventas', 'Permite crear pedidos, ver rutas y confirmar entregas')
ON CONFLICT (SystemName) DO NOTHING;

-- 5. Assign to Admin Role (RoleId = 1)
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT 1, Id FROM Permissions WHERE SystemName = 'MANAGE_ORDERS'
ON CONFLICT DO NOTHING;
