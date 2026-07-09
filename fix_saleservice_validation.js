const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.Application/Services/SaleService.cs', 'utf-8');

const validationCode = `
        // Check stock availability BEFORE creating the sale
        foreach (var detail in request.Details)
        {
            var product = await _productRepo.GetByIdAsync(detail.ProductId);
            if (product == null) throw new Exception($"Producto con ID {detail.ProductId} no encontrado.");
            
            var branchStock = await _productRepo.GetStockForBranchAsync(detail.ProductId, branchId);
            if (branchStock < detail.Quantity)
                throw new Exception($"Existencias insuficientes para el producto '{product.Name}' en esta sucursal. Stock disponible: {branchStock}");
        }
`;

if (!code.includes('Check stock availability BEFORE creating the sale')) {
    code = code.replace(
        '// 1. Insert Sale & Details',
        validationCode + '\n        // 1. Insert Sale & Details'
    );
    fs.writeFileSync('Backend/BillingSystem.Application/Services/SaleService.cs', code, 'utf-8');
    console.log("Added stock validation to SaleService");
}
