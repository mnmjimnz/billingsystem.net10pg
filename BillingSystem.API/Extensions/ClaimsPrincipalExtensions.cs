using System.Security.Claims;

namespace BillingSystem.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetRoleId(this ClaimsPrincipal user)
    {
        int.TryParse(user.FindFirst(ClaimTypes.Role)?.Value, out int roleId);
        return roleId;
    }

    public static int? GetBranchId(this ClaimsPrincipal user)
    {
        var branchIdStr = user.FindFirst("BranchId")?.Value;
        if (int.TryParse(branchIdStr, out int branchId) && branchId > 0)
            return branchId;
        return null;
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        var isAdminStr = user.FindFirst("IsAdmin")?.Value;
        if (bool.TryParse(isAdminStr, out bool isAdmin))
            return isAdmin;
        return false;
    }
}
