const fs = require('fs');
function rep(path, oldText, newText) {
    let text = fs.readFileSync(path, 'utf8');
    text = text.replace(oldText, newText);
    fs.writeFileSync(path, text);
}
rep('Backend/BillingSystem.Domain/Interfaces/IStockTransferRepository.cs', 
    'Task<IEnumerable<StockTransfer>> GetAllTransfersAsync();',
    'Task<IEnumerable<StockTransfer>> GetAllTransfersAsync();\n    Task<BillingSystem.Domain.Models.PagedResult<StockTransfer>> GetPagedAsync(int page, int pageSize);');

rep('Backend/BillingSystem.Application/Interfaces/IStockTransferService.cs', 
    'Task<IEnumerable<StockTransfer>> GetAllTransfersAsync();',
    'Task<IEnumerable<StockTransfer>> GetAllTransfersAsync();\n    Task<BillingSystem.Domain.Models.PagedResult<StockTransfer>> GetPagedTransfersAsync(int page, int pageSize);');

rep('Backend/BillingSystem.Domain/Interfaces/IBranchMovementRepository.cs',
    'Task<IEnumerable<BranchMovement>> GetByBranchIdAsync(int branchId);',
    'Task<IEnumerable<BranchMovement>> GetByBranchIdAsync(int branchId);\n    Task<BillingSystem.Domain.Models.PagedResult<BranchMovement>> GetPagedByBranchIdAsync(int branchId, int page, int pageSize);');

rep('Backend/BillingSystem.Application/Interfaces/IBranchMovementService.cs',
    'Task<IEnumerable<BranchMovement>> GetMovementsByBranchIdAsync(int branchId);',
    'Task<IEnumerable<BranchMovement>> GetMovementsByBranchIdAsync(int branchId);\n    Task<BillingSystem.Domain.Models.PagedResult<BranchMovement>> GetPagedMovementsByBranchIdAsync(int branchId, int page, int pageSize);');

console.log("Done");
