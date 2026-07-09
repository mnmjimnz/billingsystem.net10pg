const fs = require('fs');
let code = fs.readFileSync('Frontend/pages/reports.js', 'utf-8');

const filterListenersOld = `    const filterInputs = ['filterStartDate', 'filterEndDate', 'filterBranchId', 'filterUserId'];
    filterInputs.forEach(id => {
        document.getElementById(id).addEventListener('change', reloadCurrentTab);
    });`;

const filterListenersNew = `    const filterInputs = ['filterStartDate', 'filterEndDate', 'filterBranchId', 'filterUserId'];
    filterInputs.forEach(id => {
        document.getElementById(id).addEventListener('change', reloadCurrentTab);
    });

    const startInput = document.getElementById('filterStartDate');
    const endInput = document.getElementById('filterEndDate');
    
    startInput.addEventListener('change', (e) => {
        endInput.min = e.target.value;
        if (endInput.value && endInput.value < e.target.value) {
            endInput.value = e.target.value;
            reloadCurrentTab();
        }
    });

    endInput.addEventListener('change', (e) => {
        startInput.max = e.target.value;
        if (startInput.value && startInput.value > e.target.value) {
            startInput.value = e.target.value;
            reloadCurrentTab();
        }
    });
`;

code = code.replace(filterListenersOld, filterListenersNew);

const oldParams = `    if (start) {
        // Convert to UTC ISO string to ensure backend compares correctly regardless of timezone
        const startDateUtc = new Date(start + 'T00:00:00').toISOString();
        query += \`startDate=\${startDateUtc}&\`;
    }
    if (end) {
        const endDateUtc = new Date(end + 'T23:59:59').toISOString();
        query += \`endDate=\${endDateUtc}&\`;
    }`;

const newParams = `    if (start) {
        const startDateUtc = encodeURIComponent(new Date(start + 'T00:00:00').toISOString());
        query += \`startDate=\${startDateUtc}&\`;
    }
    if (end) {
        const endDateUtc = encodeURIComponent(new Date(end + 'T23:59:59').toISOString());
        query += \`endDate=\${endDateUtc}&\`;
    }`;

code = code.replace(oldParams, newParams);

fs.writeFileSync('Frontend/pages/reports.js', code, 'utf-8');
console.log("Updated reports.js date validations");
