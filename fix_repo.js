const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.Infrastructure/Repositories/StockTransferRepository.cs', 'utf-8');

const oldSql = `
            SELECT st.*, p.Name as ProductName, fb.Name as FromBranchName, tb.Name as ToBranchName, u.FullName as UserName
            FROM StockTransfers st
            JOIN Products p ON st.ProductId = p.Id
            JOIN Branches fb ON st.FromBranchId = fb.Id
            JOIN Branches tb ON st.ToBranchId = tb.Id
            JOIN Users u ON st.UserId = u.Id
            ORDER BY st.CreatedAt DESC
        ";
        
        return await connection.QueryAsync<StockTransfer, Product, Branch, Branch, User, StockTransfer>(
            sql,
            (st, p, fb, tb, u) => 
            {
                st.Product = p;
                st.FromBranch = fb;
                st.ToBranch = tb;
                st.User = u;
                return st;
            },
            splitOn: "ProductName,FromBranchName,ToBranchName,UserName"
`;

const newSql = `
            SELECT st.*, p.Name, fb.Name, tb.Name, u.FullName
            FROM StockTransfers st
            JOIN Products p ON st.ProductId = p.Id
            JOIN Branches fb ON st.FromBranchId = fb.Id
            JOIN Branches tb ON st.ToBranchId = tb.Id
            JOIN Users u ON st.UserId = u.Id
            ORDER BY st.CreatedAt DESC
        ";
        
        return await connection.QueryAsync<StockTransfer, Product, Branch, Branch, User, StockTransfer>(
            sql,
            (st, p, fb, tb, u) => 
            {
                st.Product = p;
                st.FromBranch = fb;
                st.ToBranch = tb;
                st.User = u;
                return st;
            },
            splitOn: "Name,Name,Name,FullName"
`;

code = code.replace(oldSql, newSql);
fs.writeFileSync('Backend/BillingSystem.Infrastructure/Repositories/StockTransferRepository.cs', code, 'utf-8');
console.log("Fixed Repository SQL");
