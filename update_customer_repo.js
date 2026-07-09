const fs = require('fs');

let repo = fs.readFileSync('Backend/BillingSystem.Infrastructure/Repositories/CustomerRepository.cs', 'utf8');

repo = repo.replace(
    'INSERT INTO Customers (Name, DocumentNumber, Email, Phone, Address) VALUES (@Name, @DocumentNumber, @Email, @Phone, @Address) RETURNING Id;',
    'INSERT INTO Customers (Name, DocumentNumber, Email, Phone, Address, Username, PasswordHash, Latitude, Longitude) VALUES (@Name, @DocumentNumber, @Email, @Phone, @Address, @Username, @PasswordHash, @Latitude, @Longitude) RETURNING Id;'
);

repo = repo.replace(
    'UPDATE Customers SET Name=@Name, DocumentNumber=@DocumentNumber, Email=@Email, Phone=@Phone, Address=@Address, UpdatedAt=CURRENT_TIMESTAMP WHERE Id=@Id;',
    'UPDATE Customers SET Name=@Name, DocumentNumber=@DocumentNumber, Email=@Email, Phone=@Phone, Address=@Address, Username=@Username, PasswordHash=@PasswordHash, Latitude=@Latitude, Longitude=@Longitude, UpdatedAt=CURRENT_TIMESTAMP WHERE Id=@Id;'
);

fs.writeFileSync('Backend/BillingSystem.Infrastructure/Repositories/CustomerRepository.cs', repo);
console.log("Updated CustomerRepository.cs");
