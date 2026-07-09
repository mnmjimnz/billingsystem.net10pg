const fs = require('fs');
let content = fs.readFileSync('Backend/BillingSystem.Domain/Interfaces/INewRepositories.cs', 'utf-8');
content = content.replace(
    'Task<IEnumerable<Notification>> GetUnreadAsync();',
    'Task<IEnumerable<Notification>> GetUnreadAsync();\n    Task<IEnumerable<Notification>> GetAllAsync();'
);
fs.writeFileSync('Backend/BillingSystem.Domain/Interfaces/INewRepositories.cs', content, 'utf-8');
