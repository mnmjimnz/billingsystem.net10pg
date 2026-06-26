using BillingSystem.Domain.Entities;

namespace BillingSystem.Domain.Interfaces;

public interface IRoleRepository : IRepository<Role>
{
    Task<IEnumerable<Permission>> GetAllPermissionsAsync();
    Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(int roleId);
    Task AssignPermissionsToRoleAsync(int roleId, IEnumerable<int> permissionIds);
}
