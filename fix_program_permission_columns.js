const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.API/Program.cs', 'utf-8');

const oldSql = `var sql = @"
            INSERT INTO Permissions (Name, Description) 
            SELECT 'MANAGE_TRANSFERS', 'Gestionar Traslados'
            WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'MANAGE_TRANSFERS');
        ";`;

const newSql = `var sql = @"
            INSERT INTO Permissions (SystemName, DisplayName, Module, Description) 
            SELECT 'MANAGE_TRANSFERS', 'Gestionar Traslados', 'Inventario', 'Permite gestionar traslados de sucursal'
            WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE SystemName = 'MANAGE_TRANSFERS');
        ";`;

code = code.replace(oldSql, newSql);
fs.writeFileSync('Backend/BillingSystem.API/Program.cs', code, 'utf-8');
console.log("Fixed SQL columns in Program.cs");
