const fs = require('fs');
let repo = fs.readFileSync('Backend/BillingSystem.Infrastructure/Repositories/CustomerRepository.cs', 'utf8');

repo = repo.replace('public interface ICustomerRepository : IRepository<Customer> {}', 'public interface ICustomerRepository : IRepository<Customer> {\n    Task<Customer?> GetByUsernameAsync(string username);\n}');

const getByUsernameMethod = `    public async Task<Customer?> GetByUsernameAsync(string username) {
        using var db = _conn.CreateConnection();
        return await db.QueryFirstOrDefaultAsync<Customer>("SELECT * FROM Customers WHERE Username = @Username;", new { Username = username });
    }\n\n`;

if (!repo.includes('GetByUsernameAsync(string username) {')) {
    let index = repo.indexOf('public async Task<int> AddAsync');
    repo = repo.substring(0, index) + getByUsernameMethod + repo.substring(index);
}

fs.writeFileSync('Backend/BillingSystem.Infrastructure/Repositories/CustomerRepository.cs', repo);
console.log("Updated CustomerRepository with GetByUsernameAsync");
