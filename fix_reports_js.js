const fs = require('fs');
let code = fs.readFileSync('Frontend/pages/reports.js', 'utf-8');

const oldGetFilterParams = `function getFilterParams() {
    const start = document.getElementById('filterStartDate').value;
    const end = document.getElementById('filterEndDate').value;
    const branch = document.getElementById('filterBranchId').value;
    const user = document.getElementById('filterUserId').value;
    
    let query = \`?\`;
    if (start) query += \`startDate=\${start}&\`;
    if (end) query += \`endDate=\${end}T23:59:59&\`;
    if (branch) query += \`branchId=\${branch}&\`;
    if (user) query += \`userId=\${user}&\`;
    
    return query;
}`;

const newGetFilterParams = `function getFilterParams() {
    const start = document.getElementById('filterStartDate').value;
    const end = document.getElementById('filterEndDate').value;
    const branch = document.getElementById('filterBranchId').value;
    const user = document.getElementById('filterUserId').value;
    
    let query = \`?\`;
    if (start) {
        // Convert to UTC ISO string to ensure backend compares correctly regardless of timezone
        const startDateUtc = new Date(start + 'T00:00:00').toISOString();
        query += \`startDate=\${startDateUtc}&\`;
    }
    if (end) {
        const endDateUtc = new Date(end + 'T23:59:59').toISOString();
        query += \`endDate=\${endDateUtc}&\`;
    }
    if (branch) query += \`branchId=\${branch}&\`;
    if (user) query += \`userId=\${user}&\`;
    
    return query;
}`;

code = code.replace(oldGetFilterParams, newGetFilterParams);
fs.writeFileSync('Frontend/pages/reports.js', code, 'utf-8');
console.log("Updated reports.js getFilterParams to use UTC times");
