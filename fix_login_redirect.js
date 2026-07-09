const fs = require('fs');
let content = fs.readFileSync('Frontend/pages/login.js', 'utf-8');

const oldLogic = `                localStorage.setItem('userRole', data.user.role);
                localStorage.setItem('userName', data.user.name);
                localStorage.setItem('userId', data.user.id);
                localStorage.setItem('userPermissions', JSON.stringify(data.permissions || []));
                window.location.href = 'pos.html';`;

const newLogic = `                const roleId = data.user.roleId || data.user.role;
                const permissions = data.permissions || [];
                
                localStorage.setItem('userRole', roleId);
                localStorage.setItem('userName', data.user.name || data.user.fullName || data.user.username);
                localStorage.setItem('userId', data.user.id);
                localStorage.setItem('userPermissions', JSON.stringify(permissions));
                
                if (roleId === 1 || roleId === '1' || permissions.includes('ADMIN_ALL')) {
                    window.location.href = 'pos.html';
                } else {
                    const routes = [
                        { url: 'pos.html', perm: 'MANAGE_POS' },
                        { url: 'purchases.html', perm: 'MANAGE_PURCHASES' },
                        { url: 'receivables.html', perm: 'MANAGE_RECEIVABLES' },
                        { url: 'payables.html', perm: 'MANAGE_PAYABLES' },
                        { url: 'reports.html', perm: 'VIEW_REPORTS' },
                        { url: 'products.html', perm: 'MANAGE_PRODUCTS' },
                        { url: 'kardex.html', perm: 'VIEW_KARDEX' },
                        { url: 'categories.html', perm: 'MANAGE_CATEGORIES' },
                        { url: 'customers.html', perm: 'MANAGE_CUSTOMERS' },
                        { url: 'suppliers.html', perm: 'MANAGE_SUPPLIERS' },
                        { url: 'users.html', perm: 'MANAGE_USERS' },
                        { url: 'roles.html', perm: 'MANAGE_ROLES' },
                        { url: 'branches.html', perm: 'MANAGE_BRANCHES' },
                        { url: 'branch-movements.html', perm: 'MANAGE_MOVEMENTS' },
                        { url: 'settings.html', perm: 'MANAGE_SETTINGS' }
                    ];

                    const firstRoute = routes.find(r => permissions.includes(r.perm));
                    
                    if (firstRoute) {
                        window.location.href = firstRoute.url;
                    } else {
                        window.location.href = 'unauthorized.html';
                    }
                }`;

content = content.replace(oldLogic, newLogic);
fs.writeFileSync('Frontend/pages/login.js', content, 'utf-8');
