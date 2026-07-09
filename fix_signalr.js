const fs = require('fs');
let content = fs.readFileSync('Frontend/assets/js/signalr-client.js', 'utf-8');

const oldLogic = `        notifs.forEach(n => {
            list.innerHTML += \`
                <li>
                    <a class="dropdown-item border-bottom py-2" href="receivables.html">
                        <div class="fw-bold">\${n.title}</div>
                        <small class="text-wrap text-muted">\${n.message}</small>
                    </a>
                </li>\`;
        });`;

const newLogic = `        const displayNotifs = notifs.slice(0, 5);
        displayNotifs.forEach(n => {
            list.innerHTML += \`
                <li>
                    <a class="dropdown-item border-bottom py-2" href="\${n.type === 'WARNING' ? 'payables.html' : 'receivables.html'}">
                        <div class="fw-bold">\${n.title}</div>
                        <small class="text-wrap text-muted">\${n.message}</small>
                    </a>
                </li>\`;
        });
        
        list.innerHTML += \`
            <li>
                <a class="dropdown-item text-center text-primary fw-bold py-2" href="notifications.html">
                    Ver todas las notificaciones
                </a>
            </li>\`;`;

content = content.replace(oldLogic, newLogic);
fs.writeFileSync('Frontend/assets/js/signalr-client.js', content, 'utf-8');
