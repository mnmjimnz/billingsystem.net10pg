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
