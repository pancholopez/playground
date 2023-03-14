using Microsoft.AspNetCore.Identity;

namespace Auth.Playground.AspIdentityApp.Services;

/// <inheritdoc />
// ReSharper disable once ClassNeverInstantiated.Global
public class DoesNotContainPasswordValidator<TUser>:IPasswordValidator<TUser> where TUser : class
{
    public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
    {
        var username = await manager.GetUserNameAsync(user);

        if (password.Contains(username, StringComparison.InvariantCultureIgnoreCase))
        {
            return IdentityResult.Failed(new IdentityError { Description = "Password cannot contain username" });
        }

        if (password.Contains("password", StringComparison.InvariantCultureIgnoreCase))
        {
            return IdentityResult.Failed(new IdentityError { Description = "Password cannot contain password" });
        }
        
        return IdentityResult.Success;
    }
}