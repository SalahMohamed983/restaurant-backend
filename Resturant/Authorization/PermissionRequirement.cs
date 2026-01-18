using Microsoft.AspNetCore.Authorization;

namespace Resturant.Authorization
{
    /// <summary>
    /// Authorization requirement for permission-based access control
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string PermissionCode { get; }

        public PermissionRequirement(string permissionCode)
        {
            PermissionCode = permissionCode ?? throw new ArgumentNullException(nameof(permissionCode));
        }
    }
}