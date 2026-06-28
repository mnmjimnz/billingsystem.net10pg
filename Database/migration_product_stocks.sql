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
