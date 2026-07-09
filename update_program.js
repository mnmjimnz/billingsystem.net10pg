const fs = require('fs');
let repo = fs.readFileSync('Backend/BillingSystem.Api/Program.cs', 'utf8');
if (!repo.includes('app.UseStaticFiles();')) {
    repo = repo.replace('app.UseCors("AllowAll");\r\napp.UseHttpsRedirection();', 'app.UseCors("AllowAll");\r\napp.UseStaticFiles();\r\napp.UseHttpsRedirection();');
    repo = repo.replace('app.UseCors("AllowAll");\napp.UseHttpsRedirection();', 'app.UseCors("AllowAll");\napp.UseStaticFiles();\napp.UseHttpsRedirection();');
    fs.writeFileSync('Backend/BillingSystem.Api/Program.cs', repo);
    console.log("Updated Program.cs");
}
