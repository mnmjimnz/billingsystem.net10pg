const fs = require('fs');

let repo = fs.readFileSync('Backend/BillingSystem.Infrastructure/Repositories/CustomerRepository.cs', 'utf8');

const getByUsernameMethod = `    public async Task<Customer?> GetByUsernameAsync(string username) {
        using var db = _conn.CreateConnection();
        return await db.QueryFirstOrDefaultAsync<Customer>("SELECT * FROM Customers WHERE Username = @Username AND IsActive = TRUE;", new { Username = username });
    }\n\n`;

if (!repo.includes('GetByUsernameAsync')) {
    let index = repo.indexOf('public async Task<int> AddAsync');
    repo = repo.substring(0, index) + getByUsernameMethod + repo.substring(index);
    fs.writeFileSync('Backend/BillingSystem.Infrastructure/Repositories/CustomerRepository.cs', repo);
    console.log("Added GetByUsernameAsync to CustomerRepository.cs");
} else {
    console.log("Method already exists");
}

let interfaceRepo = fs.readFileSync('Backend/BillingSystem.Domain/Interfaces/ICustomerRepository.cs', 'utf8');
if (!interfaceRepo.includes('GetByUsernameAsync')) {
    interfaceRepo = interfaceRepo.replace('}', '    Task<Customer?> GetByUsernameAsync(string username);\n}');
    fs.writeFileSync('Backend/BillingSystem.Domain/Interfaces/ICustomerRepository.cs', interfaceRepo);
    console.log("Added GetByUsernameAsync to ICustomerRepository.cs");
}
