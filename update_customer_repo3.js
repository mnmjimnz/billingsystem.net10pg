const fs = require('fs');
let repo = fs.readFileSync('Backend/BillingSystem.Infrastructure/Repositories/CustomerRepository.cs', 'utf8');

repo = repo.replace('public interface ICustomerRepository : IRepository<Customer> {\n    Task<Customer?> GetByUsernameAsync(string username);\n}', '');
repo = repo.replace('public interface ICustomerRepository : IRepository<Customer> {}', '');

fs.writeFileSync('Backend/BillingSystem.Infrastructure/Repositories/CustomerRepository.cs', repo);
console.log("Cleaned up CustomerRepository.cs");
