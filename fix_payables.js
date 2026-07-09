const fs = require('fs');
let content = fs.readFileSync('Backend/BillingSystem.API/Controllers/PayablesController.cs', 'utf-8');

// 1. Add namespace if missing (BillingSystem.Domain.Interfaces should be there)
if (!content.includes('using BillingSystem.Domain.Interfaces;')) {
    content = 'using BillingSystem.Domain.Interfaces;\n' + content;
}

// 2. Inject INotificationRepository
content = content.replace(
    'private readonly IBranchRepository _branchRepo;',
    'private readonly IBranchRepository _branchRepo;\n    private readonly INotificationRepository _notifRepo;'
);
content = content.replace(
    'public PayablesController(IPayableRepository repo, IBranchRepository branchRepo)',
    'public PayablesController(IPayableRepository repo, IBranchRepository branchRepo, INotificationRepository notifRepo)'
);
content = content.replace(
    '_branchRepo = branchRepo;',
    '_branchRepo = branchRepo;\n        _notifRepo = notifRepo;'
);

// 3. Mark notification as resolved if paid in full
const oldPaymentLogic = `        await _repo.UpdateAccountBalanceAsync(id, payment.Amount);\n\n        branch.AvailableFunds -= payment.Amount;\n        await _branchRepo.UpdateAsync(branch);`;
const newPaymentLogic = `        await _repo.UpdateAccountBalanceAsync(id, payment.Amount);\n\n        if (payment.Amount == account.Balance)\n        {\n            await _notifRepo.MarkResolvedAsync(account.PurchaseId, "WARNING");\n        }\n\n        branch.AvailableFunds -= payment.Amount;\n        await _branchRepo.UpdateAsync(branch);`;

content = content.replace(oldPaymentLogic, newPaymentLogic);
fs.writeFileSync('Backend/BillingSystem.API/Controllers/PayablesController.cs', content, 'utf-8');
