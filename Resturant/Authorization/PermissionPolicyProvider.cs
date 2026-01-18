using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Resturant.Authorization
{
    /// <summary>
    /// Provides authorization policies dynamically for permissions
    /// This allows us to use [RequirePermission("PERMISSION_CODE")] without pre-registering each policy
    /// </summary>
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // Check if this is a permission-based policy
            if (policyName.StartsWith("Permission:", StringComparison.OrdinalIgnoreCase))
            {
                var permissionCode = policyName.Substring("Permission:".Length);
                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(permissionCode))
                    .Build();
                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            // Fallback to default policy provider
            return _fallbackPolicyProvider.GetPolicyAsync(policyName);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return _fallbackPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return _fallbackPolicyProvider.GetFallbackPolicyAsync();
        }
    }
}