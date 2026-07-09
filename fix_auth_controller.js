const fs = require('fs');
let content = fs.readFileSync('Backend/BillingSystem.API/Controllers/AuthController.cs', 'utf-8');

// Add field
content = content.replace(
    'private readonly IUserRepository _userRepository;',
    'private readonly IUserRepository _userRepository;\n    private readonly IRoleRepository _roleRepository;'
);

// Add to constructor
content = content.replace(
    'public AuthController(IUserRepository userRepository, IConfiguration configuration)',
    'public AuthController(IUserRepository userRepository, IRoleRepository roleRepository, IConfiguration configuration)'
);
content = content.replace(
    '_userRepository = userRepository;',
    '_userRepository = userRepository;\n        _roleRepository = roleRepository;'
);

// Add to login
content = content.replace(
    'var token = GenerateJwtToken(user);\n        return Ok(new { token, user = new { user.Id, user.Username, user.FullName, user.RoleId, user.BranchId } });',
    'var token = GenerateJwtToken(user);\n        var perms = await _roleRepository.GetPermissionsByRoleIdAsync(user.RoleId);\n        var permissions = perms.Select(p => p.SystemName).ToList();\n        return Ok(new { token, user = new { user.Id, user.Username, user.FullName, user.RoleId, user.BranchId }, permissions });'
);

fs.writeFileSync('Backend/BillingSystem.API/Controllers/AuthController.cs', content, 'utf-8');
