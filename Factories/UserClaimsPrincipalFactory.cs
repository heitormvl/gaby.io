using System.Security.Claims;
using Gaby.io.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Gaby.io.Factories;

public class UserClaimsPrincipalFactory : UserClaimsPrincipalFactory<UserModel, IdentityRole<string>>
{
    public UserClaimsPrincipalFactory(
        UserManager<UserModel> userManager,
        RoleManager<IdentityRole<string>> roleManager,
        IOptions<IdentityOptions> options)
        : base(userManager, roleManager, options)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(UserModel user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        
        identity.AddClaim(new Claim("DisplayName", user.DisplayName));

        return identity;
    }
}
