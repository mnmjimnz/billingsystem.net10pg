const fs = require('fs');
let code = fs.readFileSync('Backend/BillingSystem.Domain/Entities/InventoryMovement.cs', 'utf-8');

if (!code.includes('BranchId')) {
    code = code.replace(
        'public int ProductId { get; set; }',
        'public int ProductId { get; set; }\n    public int? BranchId { get; set; }\n    public Branch? Branch { get; set; }'
    );
    fs.writeFileSync('Backend/BillingSystem.Domain/Entities/InventoryMovement.cs', code, 'utf-8');
    console.log("Updated InventoryMovement.cs");
}
