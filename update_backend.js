const fs = require('fs');

// 1. StockTransferRepository
let stRepo = fs.readFileSync('Backend/BillingSystem.Infrastructure/Repositories/StockTransferRepository.cs', 'utf8');
stRepo = stRepo.replace('}', `
    public async Task<BillingSystem.Domain.Models.PagedResult<StockTransfer>> GetPagedAsync(int page, int pageSize)
    {
        using var connection = _db.CreateConnection();
        var offset = (page - 1) * pageSize;
        
        var countSql = "SELECT COUNT(*) FROM StockTransfers";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql);
        
        var dataSql = @"
            SELECT st.*, 
                   fb.Name as FromBranchName,
                   tb.Name as ToBranchName,
                   p.Name as ProductName
            FROM StockTransfers st
            JOIN Branches fb ON st.FromBranchId = fb.Id
            JOIN Branches tb ON st.ToBranchId = tb.Id
            JOIN Products p ON st.ProductId = p.Id
            ORDER BY st.TransferDate DESC
            LIMIT @Limit OFFSET @Offset";
            
        var items = await connection.QueryAsync<StockTransfer>(dataSql, new { Limit = pageSize, Offset = offset });
        
        return new BillingSystem.Domain.Models.PagedResult<StockTransfer>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}`);
fs.writeFileSync('Backend/BillingSystem.Infrastructure/Repositories/StockTransferRepository.cs', stRepo);

// 2. BranchMovementRepository
let bmRepo = fs.readFileSync('Backend/BillingSystem.Infrastructure/Repositories/BranchMovementRepository.cs', 'utf8');
bmRepo = bmRepo.replace('}', `
    public async Task<BillingSystem.Domain.Models.PagedResult<BranchMovement>> GetPagedByBranchIdAsync(int branchId, int page, int pageSize)
    {
        using var connection = _connectionFactory.CreateConnection();
        var offset = (page - 1) * pageSize;
        
        var countSql = "SELECT COUNT(*) FROM BranchMovements WHERE BranchId = @BranchId AND IsActive = TRUE";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { BranchId = branchId });
        
        var dataSql = "SELECT * FROM BranchMovements WHERE BranchId = @BranchId AND IsActive = TRUE ORDER BY Date DESC LIMIT @Limit OFFSET @Offset";
        var items = await connection.QueryAsync<BranchMovement>(dataSql, new { BranchId = branchId, Limit = pageSize, Offset = offset });
        
        return new BillingSystem.Domain.Models.PagedResult<BranchMovement>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}`);
fs.writeFileSync('Backend/BillingSystem.Infrastructure/Repositories/BranchMovementRepository.cs', bmRepo);

// 3. StockTransferService
let stSvc = fs.readFileSync('Backend/BillingSystem.Application/Services/StockTransferService.cs', 'utf8');
stSvc = stSvc.replace('}', `
    public async Task<BillingSystem.Domain.Models.PagedResult<StockTransfer>> GetPagedTransfersAsync(int page, int pageSize)
    {
        return await _repository.GetPagedAsync(page, pageSize);
    }
}`);
fs.writeFileSync('Backend/BillingSystem.Application/Services/StockTransferService.cs', stSvc);

// 4. BranchMovementService
let bmSvc = fs.readFileSync('Backend/BillingSystem.Application/Services/BranchMovementService.cs', 'utf8');
bmSvc = bmSvc.replace('}', `
    public async Task<BillingSystem.Domain.Models.PagedResult<BranchMovement>> GetPagedMovementsByBranchIdAsync(int branchId, int page, int pageSize)
    {
        return await _repository.GetPagedByBranchIdAsync(branchId, page, pageSize);
    }
}`);
fs.writeFileSync('Backend/BillingSystem.Application/Services/BranchMovementService.cs', bmSvc);

// 5. StockTransfersController
let stCtrl = fs.readFileSync('Backend/BillingSystem.Api/Controllers/StockTransfersController.cs', 'utf8');
stCtrl = stCtrl.replace('public async Task<IActionResult> GetAll()', `[HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var transfers = await _transferService.GetPagedTransfersAsync(page, pageSize);
        return Ok(transfers);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()`);
fs.writeFileSync('Backend/BillingSystem.Api/Controllers/StockTransfersController.cs', stCtrl);

// 6. BranchMovementsController
let bmCtrl = fs.readFileSync('Backend/BillingSystem.Api/Controllers/BranchMovementsController.cs', 'utf8');
bmCtrl = bmCtrl.replace('public async Task<IActionResult> GetByBranch(int branchId)', `[HttpGet("branch/{branchId}/paged")]
    public async Task<IActionResult> GetPagedByBranch(int branchId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var movements = await _movementService.GetPagedMovementsByBranchIdAsync(branchId, page, pageSize);
        return Ok(movements);
    }

    [HttpGet("branch/{branchId}")]
    public async Task<IActionResult> GetByBranch(int branchId)`);
fs.writeFileSync('Backend/BillingSystem.Api/Controllers/BranchMovementsController.cs', bmCtrl);

console.log("Backend updated successfully.");
