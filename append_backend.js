const fs = require('fs');

function appendToClass(path, newCode) {
    let text = fs.readFileSync(path, 'utf8');
    let lastBrace = text.lastIndexOf('}');
    if (lastBrace !== -1) {
        text = text.substring(0, lastBrace) + newCode + '\n' + text.substring(lastBrace);
        fs.writeFileSync(path, text);
    }
}

// 1. StockTransferRepository
appendToClass('Backend/BillingSystem.Infrastructure/Repositories/StockTransferRepository.cs', `
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
    }`);

// 2. BranchMovementRepository
appendToClass('Backend/BillingSystem.Infrastructure/Repositories/BranchMovementRepository.cs', `
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
    }`);

// 3. StockTransferService
appendToClass('Backend/BillingSystem.Application/Services/StockTransferService.cs', `
    public async Task<BillingSystem.Domain.Models.PagedResult<StockTransfer>> GetPagedTransfersAsync(int page, int pageSize)
    {
        return await _transferRepo.GetPagedAsync(page, pageSize);
    }`);

// 4. BranchMovementService
appendToClass('Backend/BillingSystem.Application/Services/BranchMovementService.cs', `
    public async Task<BillingSystem.Domain.Models.PagedResult<BranchMovement>> GetPagedMovementsByBranchIdAsync(int branchId, int page, int pageSize)
    {
        return await _movementRepository.GetPagedByBranchIdAsync(branchId, page, pageSize);
    }`);

// 5. StockTransfersController
appendToClass('Backend/BillingSystem.Api/Controllers/StockTransfersController.cs', `
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var transfers = await _transferService.GetPagedTransfersAsync(page, pageSize);
        return Ok(transfers);
    }`);

// 6. BranchMovementsController
appendToClass('Backend/BillingSystem.Api/Controllers/BranchMovementsController.cs', `
    [HttpGet("branch/{branchId}/paged")]
    public async Task<IActionResult> GetPagedByBranch(int branchId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var movements = await _movementService.GetPagedMovementsByBranchIdAsync(branchId, page, pageSize);
        return Ok(movements);
    }`);

console.log("Backend updated successfully by appending.");
