const fs = require('fs');
let content = fs.readFileSync('Backend/BillingSystem.API/Controllers/PurchasesController.cs', 'utf-8');
content = content.replace(
    '[HttpGet("paged")]',
    `[HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var purchaseData = await _purchaseService.GetPurchaseWithDetailsAsync(id); // Wait, PurchaseService doesn't have it
        if (purchaseData == null) return NotFound("Compra no encontrada");
        return Ok(purchaseData);
    }

    [HttpGet("paged")]`
);
fs.writeFileSync('Backend/BillingSystem.API/Controllers/PurchasesController.cs', content, 'utf-8');
