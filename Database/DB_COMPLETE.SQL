-- PostgreSQL Init Script

CREATE TABLE Roles (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    Description VARCHAR(255),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE Branches (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Address VARCHAR(255),
    Phone VARCHAR(50),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE Users (
    Id SERIAL PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    FullName VARCHAR(100) NOT NULL,
    RoleId INT REFERENCES Roles(Id),
    BranchId INT REFERENCES Branches(Id),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE Categories (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Description VARCHAR(255),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE Products (
    Id SERIAL PRIMARY KEY,
    Barcode VARCHAR(100) UNIQUE,
    Name VARCHAR(100) NOT NULL,
    Description TEXT,
    Price DECIMAL(12,2) NOT NULL,
    Cost DECIMAL(12,2) NOT NULL,
    Stock INT NOT NULL DEFAULT 0,
    CategoryId INT REFERENCES Categories(Id),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE Customers (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    DocumentNumber VARCHAR(50),
    Email VARCHAR(100),
    Phone VARCHAR(50),
    Address VARCHAR(255),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE Suppliers (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    DocumentNumber VARCHAR(50),
    ContactName VARCHAR(100),
    Email VARCHAR(100),
    Phone VARCHAR(50),
    Address VARCHAR(255),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE Sales (
    Id SERIAL PRIMARY KEY,
    TicketNumber VARCHAR(50) NOT NULL UNIQUE,
    Date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CustomerId INT REFERENCES Customers(Id),
    UserId INT REFERENCES Users(Id),
    BranchId INT REFERENCES Branches(Id),
    Subtotal DECIMAL(12,2) NOT NULL,
    Discount DECIMAL(12,2) NOT NULL DEFAULT 0,
    Total DECIMAL(12,2) NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE SaleDetails (
    Id SERIAL PRIMARY KEY,
    SaleId INT REFERENCES Sales(Id),
    ProductId INT REFERENCES Products(Id),
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(12,2) NOT NULL,
    Subtotal DECIMAL(12,2) NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE Purchases (
    Id SERIAL PRIMARY KEY,
    InvoiceNumber VARCHAR(50) NOT NULL,
    Date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    SupplierId INT REFERENCES Suppliers(Id),
    UserId INT REFERENCES Users(Id),
    Total DECIMAL(12,2) NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE PurchaseDetails (
    Id SERIAL PRIMARY KEY,
    PurchaseId INT REFERENCES Purchases(Id),
    ProductId INT REFERENCES Products(Id),
    Quantity INT NOT NULL,
    UnitCost DECIMAL(12,2) NOT NULL,
    Subtotal DECIMAL(12,2) NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE InventoryMovements (
    Id SERIAL PRIMARY KEY,
    ProductId INT REFERENCES Products(Id),
    MovementType VARCHAR(10) NOT NULL, -- IN, OUT
    ReferenceType VARCHAR(20) NOT NULL, -- SALE, PURCHASE, ADJUSTMENT
    ReferenceId INT,
    Quantity INT NOT NULL,
    PreviousStock INT NOT NULL,
    NewStock INT NOT NULL,
    Description VARCHAR(255),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE CashRegisters (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    BranchId INT REFERENCES Branches(Id),
    Description VARCHAR(255),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE CashRegisterSessions (
    Id SERIAL PRIMARY KEY,
    CashRegisterId INT REFERENCES CashRegisters(Id),
    UserId INT REFERENCES Users(Id),
    OpeningTime TIMESTAMP NOT NULL,
    ClosingTime TIMESTAMP,
    OpeningBalance DECIMAL(12,2) NOT NULL,
    ClosingBalance DECIMAL(12,2),
    DeclaredBalance DECIMAL(12,2),
    Status VARCHAR(20) NOT NULL DEFAULT 'OPEN',
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE JournalEntries (
    Id SERIAL PRIMARY KEY,
    Date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description VARCHAR(255) NOT NULL,
    ReferenceType VARCHAR(20),
    ReferenceId INT,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE JournalEntryDetails (
    Id SERIAL PRIMARY KEY,
    JournalEntryId INT REFERENCES JournalEntries(Id),
    AccountCode VARCHAR(20) NOT NULL,
    AccountName VARCHAR(100) NOT NULL,
    Debit DECIMAL(12,2) NOT NULL DEFAULT 0,
    Credit DECIMAL(12,2) NOT NULL DEFAULT 0,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

-- Insert default data
INSERT INTO Roles (Name, Description) VALUES ('Admin', 'System Administrator'), ('Cashier', 'Point of Sale Cashier');
INSERT INTO Branches (Name, Address) VALUES ('Main Branch', '123 Main St');
-- Password hash for 'admin' (using a dummy hash for now, needs proper bcrypt hashing later)
INSERT INTO Users (Username, PasswordHash, FullName, RoleId, BranchId) VALUES ('admin', 'admin_hash_placeholder', 'Admin User', 1, 1);
ALTER TABLE Sales ADD COLUMN IF NOT EXISTS PaymentType VARCHAR(20) NOT NULL DEFAULT 'CASH';
ALTER TABLE Sales ADD COLUMN IF NOT EXISTS AmountTendered DECIMAL(12,2) NOT NULL DEFAULT 0;
ALTER TABLE Sales ADD COLUMN IF NOT EXISTS Change DECIMAL(12,2) NOT NULL DEFAULT 0;
ALTER TABLE Sales ADD COLUMN IF NOT EXISTS Status VARCHAR(20) NOT NULL DEFAULT 'PAID';

CREATE TABLE IF NOT EXISTS AccountsReceivable (
    Id SERIAL PRIMARY KEY,
    SaleId INT REFERENCES Sales(Id),
    CustomerId INT REFERENCES Customers(Id),
    TotalDebt DECIMAL(12,2) NOT NULL,
    AmountPaid DECIMAL(12,2) NOT NULL DEFAULT 0,
    Balance DECIMAL(12,2) NOT NULL,
    DueDate TIMESTAMP,
    Status VARCHAR(20) NOT NULL DEFAULT 'PENDING',
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS ReceivablePayments (
    Id SERIAL PRIMARY KEY,
    AccountId INT REFERENCES AccountsReceivable(Id),
    UserId INT REFERENCES Users(Id),
    Amount DECIMAL(12,2) NOT NULL,
    PaymentDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Notes VARCHAR(255),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS Notifications (
    Id SERIAL PRIMARY KEY,
    Title VARCHAR(100) NOT NULL,
    Message VARCHAR(255) NOT NULL,
    Type VARCHAR(20) NOT NULL DEFAULT 'INFO',
    ReferenceId INT,
    IsResolved BOOLEAN NOT NULL DEFAULT FALSE,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);
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
-- Create ProductStocks table
CREATE TABLE IF NOT EXISTS ProductStocks (
    Id SERIAL PRIMARY KEY,
    ProductId INT NOT NULL REFERENCES Products(Id),
    BranchId INT NOT NULL REFERENCES Branches(Id),
    Stock INT NOT NULL DEFAULT 0,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP NULL,
    UNIQUE(ProductId, BranchId)
);

-- Add BranchId to InventoryMovements
ALTER TABLE InventoryMovements ADD COLUMN IF NOT EXISTS BranchId INT NULL REFERENCES Branches(Id);

-- Create StockTransfers table
CREATE TABLE IF NOT EXISTS StockTransfers (
    Id SERIAL PRIMARY KEY,
    ProductId INT NOT NULL REFERENCES Products(Id),
    FromBranchId INT NOT NULL REFERENCES Branches(Id),
    ToBranchId INT NOT NULL REFERENCES Branches(Id),
    Quantity INT NOT NULL,
    UserId INT NOT NULL REFERENCES Users(Id),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Notes TEXT NULL
);

-- Initialize ProductStocks with existing global stock into BranchId = 1 (or the first branch)
INSERT INTO ProductStocks (ProductId, BranchId, Stock, CreatedAt, UpdatedAt)
SELECT p.Id, (SELECT MIN(Id) FROM Branches), p.Stock, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
FROM Products p
WHERE NOT EXISTS (
    SELECT 1 FROM ProductStocks ps WHERE ps.ProductId = p.Id
);

-- Ensure all existing inventory movements are assigned to the first branch if null
UPDATE InventoryMovements 
SET BranchId = (SELECT MIN(Id) FROM Branches) 
WHERE BranchId IS NULL;
-- Migration to add Credit Purchases and Accounts Payable

ALTER TABLE Purchases ADD COLUMN IF NOT EXISTS PaymentType VARCHAR(20) NOT NULL DEFAULT 'CASH';
ALTER TABLE Purchases ADD COLUMN IF NOT EXISTS AmountPaid DECIMAL(12,2) NOT NULL DEFAULT 0;
ALTER TABLE Purchases ADD COLUMN IF NOT EXISTS Status VARCHAR(20) NOT NULL DEFAULT 'PAID';

CREATE TABLE IF NOT EXISTS AccountsPayable (
    Id SERIAL PRIMARY KEY,
    PurchaseId INT REFERENCES Purchases(Id),
    SupplierId INT REFERENCES Suppliers(Id),
    TotalDebt DECIMAL(12,2) NOT NULL,
    AmountPaid DECIMAL(12,2) NOT NULL DEFAULT 0,
    Balance DECIMAL(12,2) NOT NULL,
    DueDate TIMESTAMP,
    Status VARCHAR(20) NOT NULL DEFAULT 'PENDING',
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS PayablePayments (
    Id SERIAL PRIMARY KEY,
    AccountId INT REFERENCES AccountsPayable(Id),
    UserId INT REFERENCES Users(Id),
    Amount DECIMAL(12,2) NOT NULL,
    PaymentDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Notes VARCHAR(255),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);
-- Añadir campos de RRHH a tabla Users
ALTER TABLE Users
ADD COLUMN Salary DECIMAL(12,2) NULL,
ADD COLUMN HireDate TIMESTAMP NULL,
ADD COLUMN TerminationDate TIMESTAMP NULL,
ADD COLUMN TerminationReason VARCHAR(255) NULL;

-- Crear tabla para Movimientos de Sucursal
CREATE TABLE BranchMovements (
    Id SERIAL PRIMARY KEY,
    BranchId INT REFERENCES Branches(Id) NOT NULL,
    Amount DECIMAL(12,2) NOT NULL,
    Type VARCHAR(10) NOT NULL, -- 'IN' or 'OUT'
    Category VARCHAR(100) NOT NULL, -- e.g. 'Inyección de Capital', 'Pago de Servicios', 'Nómina'
    Description VARCHAR(255),
    UserId INT REFERENCES Users(Id), -- Quien registra el movimiento
    EmployeeId INT REFERENCES Users(Id) NULL, -- Empleado relacionado (ej: pago de nómina)
    Date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);
ALTER TABLE Customers
ADD COLUMN Username VARCHAR(100),
ADD COLUMN PasswordHash VARCHAR(255),
ADD COLUMN Latitude DECIMAL(18,8),
ADD COLUMN Longitude DECIMAL(18,8);
-- Missing Tables and Columns to match API Models

-- Alter Users Table
ALTER TABLE Users
ADD COLUMN IF NOT EXISTS JobTitle VARCHAR(100),
ADD COLUMN IF NOT EXISTS DocumentId VARCHAR(50),
ADD COLUMN IF NOT EXISTS BaseBonus DECIMAL(12,2) DEFAULT 0,
ADD COLUMN IF NOT EXISTS IncomeTaxPercentage DECIMAL(5,2) DEFAULT 0,
ADD COLUMN IF NOT EXISTS IsAdmin BOOLEAN DEFAULT FALSE;

-- Chart of Accounts
CREATE TABLE IF NOT EXISTS Accounts (
    Id SERIAL PRIMARY KEY,
    Code VARCHAR(20) NOT NULL UNIQUE,
    Name VARCHAR(100) NOT NULL,
    Type VARCHAR(50) NOT NULL,
    ParentAccountId INT REFERENCES Accounts(Id),
    Level INT NOT NULL DEFAULT 1,
    AllowsTransactions BOOLEAN NOT NULL DEFAULT TRUE,
    Description VARCHAR(255),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

-- Bank Accounts
CREATE TABLE IF NOT EXISTS BankAccounts (
    Id SERIAL PRIMARY KEY,
    BankName VARCHAR(100) NOT NULL,
    AccountNumber VARCHAR(50) NOT NULL,
    Currency VARCHAR(10) NOT NULL DEFAULT 'USD',
    CurrentBalance DECIMAL(18,2) NOT NULL DEFAULT 0,
    LinkedAccountId INT REFERENCES Accounts(Id),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS BankReconciliations (
    Id SERIAL PRIMARY KEY,
    BankAccountId INT REFERENCES BankAccounts(Id),
    StatementDate TIMESTAMP NOT NULL,
    StatementBalance DECIMAL(18,2) NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'DRAFT',
    Notes TEXT,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS BankReconciliationDetails (
    Id SERIAL PRIMARY KEY,
    BankReconciliationId INT REFERENCES BankReconciliations(Id),
    JournalEntryDetailId INT, -- References JournalEntryDetails
    IsCleared BOOLEAN NOT NULL DEFAULT FALSE,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

-- Company Settings
CREATE TABLE IF NOT EXISTS CompanySettings (
    Id SERIAL PRIMARY KEY,
    CompanyName VARCHAR(100) NOT NULL DEFAULT 'Mi Empresa',
    Address VARCHAR(255),
    Phone VARCHAR(50),
    Email VARCHAR(100),
    TaxPercentage DECIMAL(5,2) NOT NULL DEFAULT 15.00,
    StoreTheme VARCHAR(50) DEFAULT 'minimalist',
    ShowStoreSlider BOOLEAN DEFAULT true,
    StoreProductsPerPage INT DEFAULT 12,
    ActiveThemeId INT,
    SliderImage1 VARCHAR(500),
    SliderImage2 VARCHAR(500),
    SliderImage3 VARCHAR(500),
    SocialSecurityPercentage DECIMAL(5,2) NOT NULL DEFAULT 0,
    AfpPercentage DECIMAL(5,2) NOT NULL DEFAULT 0,
    UpdatedAt TIMESTAMP
);

-- HR
CREATE TABLE IF NOT EXISTS Attendances (
    Id SERIAL PRIMARY KEY,
    UserId INT REFERENCES Users(Id),
    Date TIMESTAMP NOT NULL,
    CheckInTime TIME,
    CheckOutTime TIME,
    Status VARCHAR(20) NOT NULL DEFAULT 'Present',
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS PayrollRuns (
    Id SERIAL PRIMARY KEY,
    PeriodStart TIMESTAMP NOT NULL,
    PeriodEnd TIMESTAMP NOT NULL,
    ProcessedDate TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Notes TEXT,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS PayrollDetails (
    Id SERIAL PRIMARY KEY,
    PayrollRunId INT REFERENCES PayrollRuns(Id),
    UserId INT REFERENCES Users(Id),
    BaseSalary DECIMAL(12,2) NOT NULL,
    ExtraHoursAmount DECIMAL(12,2) NOT NULL DEFAULT 0,
    BonusAmount DECIMAL(12,2) NOT NULL DEFAULT 0,
    DeductionsAmount DECIMAL(12,2) NOT NULL DEFAULT 0,
    NetPay DECIMAL(12,2) NOT NULL,
    Observations TEXT,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

-- Coupons
CREATE TABLE IF NOT EXISTS Coupons (
    Id SERIAL PRIMARY KEY,
    Code VARCHAR(50) NOT NULL UNIQUE,
    DiscountPercentage DECIMAL(5,2),
    DiscountAmount DECIMAL(12,2),
    ValidFrom TIMESTAMP,
    ValidUntil TIMESTAMP,
    MaxUses INT,
    CurrentUses INT NOT NULL DEFAULT 0,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

-- BranchMovements additions
ALTER TABLE BranchMovements 
ADD COLUMN IF NOT EXISTS CashRegisterId INT,
ADD COLUMN IF NOT EXISTS AccountId INT,
ADD COLUMN IF NOT EXISTS PaymentMethod VARCHAR(20) DEFAULT 'CASH';

-- JournalEntryDetails additions
ALTER TABLE JournalEntryDetails
ADD COLUMN IF NOT EXISTS AccountId INT;

-- Other Missing Columns
ALTER TABLE Branches
ADD COLUMN IF NOT EXISTS AvailableFunds DECIMAL(12,2) DEFAULT 0,
ADD COLUMN IF NOT EXISTS Status VARCHAR(20) DEFAULT 'OPEN';

ALTER TABLE Orders
ADD COLUMN IF NOT EXISTS PaymentMethod VARCHAR(20);

ALTER TABLE OrderDetails
ADD COLUMN IF NOT EXISTS ProductName VARCHAR(100);

ALTER TABLE Products
ADD COLUMN IF NOT EXISTS IsTaxExempt BOOLEAN DEFAULT FALSE,
ADD COLUMN IF NOT EXISTS ImageUrl VARCHAR(500);

ALTER TABLE Purchases
ADD COLUMN IF NOT EXISTS BranchId INT REFERENCES Branches(Id);

ALTER TABLE Sales
ADD COLUMN IF NOT EXISTS TaxAmount DECIMAL(12,2) DEFAULT 0;

-- Fleet Management
CREATE TABLE IF NOT EXISTS Vehicles (
    Id SERIAL PRIMARY KEY,
    PlateNumber VARCHAR(20) NOT NULL,
    Model VARCHAR(100) NOT NULL,
    Capacity DECIMAL(10,2),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS Drivers (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    LicenseNumber VARCHAR(50) NOT NULL,
    Phone VARCHAR(50),
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS DeliveryRoutes (
    Id SERIAL PRIMARY KEY,
    Date TIMESTAMP NOT NULL,
    DriverId INT REFERENCES Drivers(Id),
    VehicleId INT REFERENCES Vehicles(Id),
    BranchId INT REFERENCES Branches(Id),
    Status VARCHAR(20) NOT NULL DEFAULT 'PENDING',
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS RouteStops (
    Id SERIAL PRIMARY KEY,
    DeliveryRouteId INT REFERENCES DeliveryRoutes(Id),
    OrderId INT REFERENCES Orders(Id),
    StopOrder INT NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'PENDING',
    EstimatedTime TIMESTAMP,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);
-- migration_theme_engine.sql

DROP TABLE IF EXISTS "ThemeSettings";
DROP TABLE IF EXISTS "Themes";

CREATE TABLE "Themes" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Code" VARCHAR(50) NOT NULL UNIQUE,
    "Description" TEXT,
    "PreviewImage" VARCHAR(500),
    "Version" VARCHAR(20) DEFAULT '1.0.0',
    "IsActive" BOOLEAN DEFAULT false,
    "InstalledDate" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "ThemeSettings" (
    "Id" SERIAL PRIMARY KEY,
    "ThemeId" INT NOT NULL REFERENCES "Themes"("Id") ON DELETE CASCADE,
    "PrimaryColor" VARCHAR(20) DEFAULT '#000000',
    "SecondaryColor" VARCHAR(20) DEFAULT '#ffffff',
    "FontFamily" VARCHAR(100) DEFAULT 'Inter, sans-serif',
    "BorderRadius" VARCHAR(20) DEFAULT '0px',
    "MainBannerUrl" VARCHAR(500),
    "LogoUrl" VARCHAR(500),
    "ButtonStyle" VARCHAR(20) DEFAULT 'solid',
    "ProductsPerRow" INT DEFAULT 4,
    "ProductCardStyle" VARCHAR(20) DEFAULT 'clean',
    "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Insert initial themes
INSERT INTO "Themes" ("Name", "Code", "Description", "PreviewImage") VALUES 
('Modern Store', 'modern', 'Estilo Apple/Allbirds. Minimalista, mucho espacio en blanco, enfocado en el producto.', 'https://placehold.co/600x400?text=Modern+Store'),
('Marketplace Pro', 'marketplace', 'Estilo Amazon. Optimizado para catálogos grandes, filtros avanzados y ofertas.', 'https://placehold.co/600x400?text=Marketplace+Pro'),
('Fashion Premium', 'fashion', 'Estilo Nike/KITH. Orientado a marca, imágenes inmersivas y storytelling.', 'https://placehold.co/600x400?text=Fashion+Premium'),
('Electronics Store', 'electronics', 'Estilo Samsung. Diseño tecnológico y fichas técnicas destacadas.', 'https://placehold.co/600x400?text=Electronics+Store');

-- Default settings for Modern Store
INSERT INTO "ThemeSettings" ("ThemeId", "PrimaryColor", "SecondaryColor", "FontFamily", "BorderRadius", "ProductsPerRow", "ProductCardStyle") 
VALUES ((SELECT "Id" FROM "Themes" WHERE "Code" = 'modern'), '#1d1d1f', '#f5f5f7', 'Inter, sans-serif', '0px', 3, 'clean');

-- Default settings for Marketplace Pro
INSERT INTO "ThemeSettings" ("ThemeId", "PrimaryColor", "SecondaryColor", "FontFamily", "BorderRadius", "ProductsPerRow", "ProductCardStyle") 
VALUES ((SELECT "Id" FROM "Themes" WHERE "Code" = 'marketplace'), '#232f3e', '#febd69', 'Arial, sans-serif', '4px', 5, 'bordered');

-- Default settings for Fashion Premium
INSERT INTO "ThemeSettings" ("ThemeId", "PrimaryColor", "SecondaryColor", "FontFamily", "BorderRadius", "ProductsPerRow", "ProductCardStyle") 
VALUES ((SELECT "Id" FROM "Themes" WHERE "Code" = 'fashion'), '#000000', '#ffffff', 'Helvetica Neue, sans-serif', '0px', 3, 'minimal');

-- Default settings for Electronics Store
INSERT INTO "ThemeSettings" ("ThemeId", "PrimaryColor", "SecondaryColor", "FontFamily", "BorderRadius", "ProductsPerRow", "ProductCardStyle") 
VALUES ((SELECT "Id" FROM "Themes" WHERE "Code" = 'electronics'), '#0381fe', '#000000', 'Roboto, sans-serif', '8px', 4, 'shadow');

-- Alter CompanySettings to add ActiveThemeId
ALTER TABLE companysettings ADD COLUMN IF NOT EXISTS "ActiveThemeId" INT REFERENCES "Themes"("Id");
UPDATE companysettings SET "ActiveThemeId" = (SELECT "Id" FROM "Themes" WHERE "Code" = 'modern');
-- migration_store_settings.sql
ALTER TABLE CompanySettings 
ADD COLUMN IF NOT EXISTS StoreTheme VARCHAR(50) DEFAULT 'minimalist',
ADD COLUMN IF NOT EXISTS ShowStoreSlider BOOLEAN DEFAULT true,
ADD COLUMN IF NOT EXISTS StoreProductsPerPage INT DEFAULT 12,
ADD COLUMN IF NOT EXISTS SliderImage1 VARCHAR(500) DEFAULT 'https://images.unsplash.com/photo-1607082348824-0a96f2a4b9da?q=80&w=2070&auto=format&fit=crop',
ADD COLUMN IF NOT EXISTS SliderImage2 VARCHAR(500) DEFAULT 'https://images.unsplash.com/photo-1607082350899-7e105aa886ae?q=80&w=2070&auto=format&fit=crop',
ADD COLUMN IF NOT EXISTS SliderImage3 VARCHAR(500) DEFAULT 'https://images.unsplash.com/photo-1472851294608-062f824d29cc?q=80&w=2070&auto=format&fit=crop';
