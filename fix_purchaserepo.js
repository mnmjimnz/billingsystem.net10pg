const fs = require('fs');
let interfaceContent = fs.readFileSync('Backend/BillingSystem.Domain/Interfaces/IPurchaseRepository.cs', 'utf-8');
interfaceContent = interfaceContent.replace(
    'Task<BillingSystem.Domain.Models.PagedResult<dynamic>> GetPagedAsync',
    'Task<dynamic> GetPurchaseWithDetailsAsync(int id);\n    Task<BillingSystem.Domain.Models.PagedResult<dynamic>> GetPagedAsync'
);
fs.writeFileSync('Backend/BillingSystem.Domain/Interfaces/IPurchaseRepository.cs', interfaceContent, 'utf-8');

let repoContent = fs.readFileSync('Backend/BillingSystem.Infrastructure/Repositories/PurchaseRepository.cs', 'utf-8');
repoContent = repoContent.replace(
    'public Task<Purchase?> GetByIdAsync(int id) => throw new NotImplementedException();',
    `public async Task<dynamic> GetPurchaseWithDetailsAsync(int id)
    {
        using var connection = _db.CreateConnection();
        var purchaseSql = @"SELECT p.*, s.Name as SupplierName, u.FullName as UserName, b.Name as BranchName 
                            FROM Purchases p 
                            JOIN Suppliers s ON p.SupplierId = s.Id
                            JOIN Users u ON p.UserId = u.Id
                            JOIN Branches b ON p.BranchId = b.Id
                            WHERE p.Id = @Id";
        var purchase = await connection.QueryFirstOrDefaultAsync<dynamic>(purchaseSql, new { Id = id });

        if (purchase != null)
        {
            var detailsSql = @"SELECT pd.*, pr.Name as ProductName, pr.Code as ProductCode 
                               FROM PurchaseDetails pd
                               JOIN Products pr ON pd.ProductId = pr.Id
                               WHERE pd.PurchaseId = @Id";
            var details = await connection.QueryAsync<dynamic>(detailsSql, new { Id = id });
            
            // Assigning details to a new dynamic object to return together
            return new {
                Purchase = purchase,
                Details = details
            };
        }
        return null;
    }

    public Task<Purchase?> GetByIdAsync(int id) => throw new NotImplementedException();`
);
fs.writeFileSync('Backend/BillingSystem.Infrastructure/Repositories/PurchaseRepository.cs', repoContent, 'utf-8');
