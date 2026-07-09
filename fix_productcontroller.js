const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.Api/Controllers/ProductsController.cs', 'utf-8');

const stockEndpoint = `
    [HttpGet("{id}/stock")]
    public async Task<IActionResult> GetStockByBranch(int id)
    {
        var stock = await _productRepository.GetStockByBranchAsync(id);
        return Ok(stock);
    }
`;

if (!code.includes('GetStockByBranch')) {
    code = code.replace(
        'public async Task<IActionResult> GetByBarcode(string barcode)',
        stockEndpoint + '\n    [HttpGet("barcode/{barcode}")]\n    public async Task<IActionResult> GetByBarcode(string barcode)'
    );
    fs.writeFileSync('Backend/BillingSystem.Api/Controllers/ProductsController.cs', code, 'utf-8');
    console.log("Fixed ProductsController");
}
