import os

def replace_in_file(path, old, new):
    with open(path, 'r', encoding='utf-8') as f:
        content = f.read()
    content = content.replace(old, new)
    with open(path, 'w', encoding='utf-8') as f:
        f.write(content)

# 1. IStockTransferRepository
replace_in_file('Backend/BillingSystem.Domain/Interfaces/IStockTransferRepository.cs', 
    'Task<IEnumerable<StockTransfer>> GetAllTransfersAsync();',
    'Task<IEnumerable<StockTransfer>> GetAllTransfersAsync();\n    Task<BillingSystem.Domain.Models.PagedResult<StockTransfer>> GetPagedAsync(int page, int pageSize);')

# 2. IStockTransferService
replace_in_file('Backend/BillingSystem.Application/Interfaces/IStockTransferService.cs', 
    'Task<IEnumerable<StockTransfer>> GetAllTransfersAsync();',
    'Task<IEnumerable<StockTransfer>> GetAllTransfersAsync();\n    Task<BillingSystem.Domain.Models.PagedResult<StockTransfer>> GetPagedTransfersAsync(int page, int pageSize);')

# 3. IBranchMovementRepository
replace_in_file('Backend/BillingSystem.Domain/Interfaces/IBranchMovementRepository.cs',
    'Task<IEnumerable<BranchMovement>> GetByBranchIdAsync(int branchId);',
    'Task<IEnumerable<BranchMovement>> GetByBranchIdAsync(int branchId);\n    Task<BillingSystem.Domain.Models.PagedResult<BranchMovement>> GetPagedByBranchIdAsync(int branchId, int page, int pageSize);')

# 4. IBranchMovementService
replace_in_file('Backend/BillingSystem.Application/Interfaces/IBranchMovementService.cs',
    'Task<IEnumerable<BranchMovement>> GetMovementsByBranchIdAsync(int branchId);',
    'Task<IEnumerable<BranchMovement>> GetMovementsByBranchIdAsync(int branchId);\n    Task<BillingSystem.Domain.Models.PagedResult<BranchMovement>> GetPagedMovementsByBranchIdAsync(int branchId, int page, int pageSize);')

print("Interfaces updated")
