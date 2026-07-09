const fs = require('fs');

let branchCs = fs.readFileSync('Backend/BillingSystem.Domain/Entities/Branch.cs', 'utf-8');
branchCs = branchCs.replace('public string Status { get; set; } = "OPEN";', `public string Status { get; set; } = "OPEN";
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }`);
fs.writeFileSync('Backend/BillingSystem.Domain/Entities/Branch.cs', branchCs, 'utf-8');

let repo = fs.readFileSync('Backend/BillingSystem.Infrastructure/Repositories/BranchRepository.cs', 'utf-8');
repo = repo.replace('Name, Address, Phone, AvailableFunds, Status, CreatedAt, IsActive', 'Name, Address, Phone, AvailableFunds, Status, Latitude, Longitude, CreatedAt, IsActive');
repo = repo.replace('@Name, @Address, @Phone, @AvailableFunds, @Status, @CreatedAt, @IsActive', '@Name, @Address, @Phone, @AvailableFunds, @Status, @Latitude, @Longitude, @CreatedAt, @IsActive');
repo = repo.replace('Name = @Name, Address = @Address, Phone = @Phone, AvailableFunds = @AvailableFunds, Status = @Status, UpdatedAt = @UpdatedAt, IsActive = @IsActive', 'Name = @Name, Address = @Address, Phone = @Phone, AvailableFunds = @AvailableFunds, Status = @Status, Latitude = @Latitude, Longitude = @Longitude, UpdatedAt = @UpdatedAt, IsActive = @IsActive');
fs.writeFileSync('Backend/BillingSystem.Infrastructure/Repositories/BranchRepository.cs', repo, 'utf-8');

console.log("Branch entity and repo updated");
