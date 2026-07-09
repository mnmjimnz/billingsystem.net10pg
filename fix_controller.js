const fs = require('fs');
let content = fs.readFileSync('Backend/BillingSystem.API/Controllers/NotificationsController.cs', 'utf-8');
content = content.replace(
    '[HttpGet]',
    '[HttpGet("all")]\n    public async Task<IActionResult> GetAll()\n    {\n        var notifications = await _notifRepo.GetAllAsync();\n        return Ok(notifications);\n    }\n\n    [HttpGet]'
);
fs.writeFileSync('Backend/BillingSystem.API/Controllers/NotificationsController.cs', content, 'utf-8');
