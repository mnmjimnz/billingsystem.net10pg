const fs = require('fs');

// 1. Order.cs
let orderCs = fs.readFileSync('Backend/BillingSystem.Domain/Entities/Order.cs', 'utf8');
if (!orderCs.includes('PaymentMethod')) {
    orderCs = orderCs.replace(
        /public string Status \{ get; set; \} = "PENDING";/,
        'public string Status { get; set; } = "PENDING";\n    public string PaymentMethod { get; set; } = "EFECTIVO";'
    );
    fs.writeFileSync('Backend/BillingSystem.Domain/Entities/Order.cs', orderCs);
}

// 2. OrderRepository.cs
let repoCs = fs.readFileSync('Backend/BillingSystem.Infrastructure/Repositories/OrderRepository.cs', 'utf8');
repoCs = repoCs.replace(
    /INSERT INTO Orders \(OrderNumber, Date, CustomerId, BranchId, Status, DeliveryAddress, Latitude, Longitude, Notes, Total, CreatedAt\)/,
    'INSERT INTO orders ("OrderNumber", "Date", "CustomerId", "BranchId", "Status", "DeliveryAddress", "Latitude", "Longitude", "Notes", "Total", "PaymentMethod", "CreatedAt")'
);
repoCs = repoCs.replace(
    /VALUES \(@OrderNumber, @Date, @CustomerId, @BranchId, @Status, @DeliveryAddress, @Latitude, @Longitude, @Notes, @Total, CURRENT_TIMESTAMP\)/,
    'VALUES (@OrderNumber, @Date, @CustomerId, @BranchId, @Status, @DeliveryAddress, @Latitude, @Longitude, @Notes, @Total, @PaymentMethod, CURRENT_TIMESTAMP)'
);
// Also fix table names in OrderRepository just in case they were uppercase Orders but postgres is case sensitive if quoted, otherwise it treats as lowercase.
// "INSERT INTO orders" is safer.
repoCs = repoCs.replace(/INSERT INTO Orders/g, 'INSERT INTO orders');
repoCs = repoCs.replace(/INSERT INTO OrderDetails/g, 'INSERT INTO orderdetails');
fs.writeFileSync('Backend/BillingSystem.Infrastructure/Repositories/OrderRepository.cs', repoCs);

// 3. StoreController.cs
let ctrlCs = fs.readFileSync('Backend/BillingSystem.Api/Controllers/StoreController.cs', 'utf8');
if (!ctrlCs.includes('public string PaymentMethod')) {
    ctrlCs = ctrlCs.replace(
        /public string Notes \{ get; set; \} = string\.Empty;/,
        'public string Notes { get; set; } = string.Empty;\n        public string PaymentMethod { get; set; } = "EFECTIVO";'
    );
}
// Fix AddAsync to AddOrderAsync
ctrlCs = ctrlCs.replace(
    /var orderId = await _orderRepository\.AddAsync\(order\);/,
    'var orderId = await _orderRepository.AddOrderAsync(order, order.Details);'
);
// Set PaymentMethod on order entity
ctrlCs = ctrlCs.replace(
    /Notes = request\.Notes,/,
    'Notes = request.Notes,\n            PaymentMethod = request.PaymentMethod,'
);
fs.writeFileSync('Backend/BillingSystem.Api/Controllers/StoreController.cs', ctrlCs);

console.log("Backend updated");
