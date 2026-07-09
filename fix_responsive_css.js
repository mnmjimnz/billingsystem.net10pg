const fs = require('fs');
let css = fs.readFileSync('Frontend/assets/css/devextreme-theme.css', 'utf-8');

const responsiveCSS = `
/* Responsive Utilities */
@media (min-width: 992px) {
    .d-lg-block { display: block !important; }
    .d-lg-flex { display: flex !important; }
    .d-lg-none { display: none !important; }
}
@media (min-width: 768px) {
    .d-md-block { display: block !important; }
    .d-md-flex { display: flex !important; }
    .d-md-none { display: none !important; }
}
`;

if (!css.includes('.d-lg-block')) {
    css += responsiveCSS;
    fs.writeFileSync('Frontend/assets/css/devextreme-theme.css', css, 'utf-8');
    console.log("Added responsive classes to CSS");
} else {
    console.log("Already present");
}
