const fs = require('fs');

let repo = fs.readFileSync('Backend/BillingSystem.Infrastructure/Repositories/StockTransferRepository.cs', 'utf8');

const newMethod = `    public async Task<BillingSystem.Domain.Models.PagedResult<StockTransfer>> GetPagedAsync(int page, int pageSize)
    {
        using var connection = _db.CreateConnection();
        var offset = (page - 1) * pageSize;
        
        var countSql = "SELECT COUNT(*) FROM StockTransfers";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql);
        
        var sql = @"
            SELECT st.*, p.Name, fb.Name, tb.Name, u.FullName
            FROM StockTransfers st
            JOIN Products p ON st.ProductId = p.Id
            JOIN Branches fb ON st.FromBranchId = fb.Id
            JOIN Branches tb ON st.ToBranchId = tb.Id
            JOIN Users u ON st.UserId = u.Id
            ORDER BY st.CreatedAt DESC
            LIMIT @Limit OFFSET @Offset";
            
        var items = await connection.QueryAsync<StockTransfer, Product, Branch, Branch, User, StockTransfer>(
            sql,
            (st, p, fb, tb, u) => 
            {
                st.Product = p;
                st.FromBranch = fb;
                st.ToBranch = tb;
                st.User = u;
                return st;
            },
            new { Limit = pageSize, Offset = offset });
        
        return new BillingSystem.Domain.Models.PagedResult<StockTransfer>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }`;

let startIndex = repo.indexOf('public async Task<BillingSystem.Domain.Models.PagedResult<StockTransfer>> GetPagedAsync');
if (startIndex !== -1) {
    let endIndex = repo.indexOf('}', repo.indexOf('}', repo.indexOf('PageSize = pageSize', startIndex))) + 1;
    let endIndex2 = repo.indexOf('}', endIndex) + 1; // get out of method block
    repo = repo.substring(0, startIndex) + newMethod + repo.substring(endIndex2);
    fs.writeFileSync('Backend/BillingSystem.Infrastructure/Repositories/StockTransferRepository.cs', repo);
}
console.log("Replaced GetPagedAsync in StockTransferRepository");
