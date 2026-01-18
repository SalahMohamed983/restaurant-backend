using Microsoft.AspNetCore.Authorization;
using Resturant.Authorization;
using System;

namespace Resturant.Attributes
{
    /// <summary>
    /// Authorization attribute that requires a specific permission
    /// Usage: [RequirePermission("MENU_CREATE")]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public string PermissionCode { get; }

        public RequirePermissionAttribute(string permissionCode)
        {
            PermissionCode = permissionCode ?? throw new ArgumentNullException(nameof(permissionCode));
            Policy = $"Permission:{permissionCode}";
        }
    }
}