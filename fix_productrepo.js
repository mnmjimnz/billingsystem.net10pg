const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.Infrastructure/Repositories/ProductRepository.cs', 'utf-8');

const getByBarcode = `
    public async Task<Product?> GetByBarcodeAsync(string barcode)
    {
        using var connection = _db.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Product>("SELECT * FROM Products WHERE Barcode = @Barcode", new { Barcode = barcode });
    }
`;

if (!code.includes('GetByBarcodeAsync')) {
    code = code.replace(
        'public async Task<int> AddAsync(Product entity)',
        getByBarcode + '\n    public async Task<int> AddAsync(Product entity)'
    );
    fs.writeFileSync('Backend/BillingSystem.Infrastructure/Repositories/ProductRepository.cs', code, 'utf-8');
    console.log("Fixed ProductRepository");
}
