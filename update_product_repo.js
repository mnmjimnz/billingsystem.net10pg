const fs = require('fs');
let repo = fs.readFileSync('Backend/BillingSystem.Infrastructure/Repositories/ProductRepository.cs', 'utf8');

repo = repo.replace(
    'INSERT INTO Products (Barcode, Name, Price, Cost, Stock, CategoryId, IsTaxExempt, CreatedAt, UpdatedAt)',
    'INSERT INTO Products (Barcode, Name, Price, Cost, Stock, CategoryId, IsTaxExempt, ImageUrl, CreatedAt, UpdatedAt)'
);
repo = repo.replace(
    'VALUES (@Barcode, @Name, @Price, @Cost, @Stock, @CategoryId, @IsTaxExempt, @CreatedAt, @UpdatedAt) RETURNING Id',
    'VALUES (@Barcode, @Name, @Price, @Cost, @Stock, @CategoryId, @IsTaxExempt, @ImageUrl, @CreatedAt, @UpdatedAt) RETURNING Id'
);

repo = repo.replace(
    'Stock = @Stock, CategoryId = @CategoryId, IsTaxExempt = @IsTaxExempt, UpdatedAt = @UpdatedAt',
    'Stock = @Stock, CategoryId = @CategoryId, IsTaxExempt = @IsTaxExempt, ImageUrl = @ImageUrl, UpdatedAt = @UpdatedAt'
);

repo = repo.replace(
    'IsTaxExempt = i.istaxexempt',
    'IsTaxExempt = i.istaxexempt,\n                ImageUrl = i.imageurl'
);

fs.writeFileSync('Backend/BillingSystem.Infrastructure/Repositories/ProductRepository.cs', repo);
console.log("Updated ProductRepository.cs");
