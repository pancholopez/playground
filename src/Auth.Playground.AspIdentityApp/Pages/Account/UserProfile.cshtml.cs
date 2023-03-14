using Auth.Playground.AspIdentityApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Auth.Playground.AspIdentityApp.Pages.Account;

[Authorize]
public class UserProfileModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    [BindProperty]
    public UserProfileViewModel ViewModel { get; set; } = new();

    [BindProperty] 
    public string SuccessMessage { get; set; } = string.Empty;

    public UserProfileModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var (user, departmentClaim, positionClaim) = await GetUserInfo();

        ViewModel.Email = user.Email;
        ViewModel.Department = departmentClaim.Value;
        ViewModel.Position = positionClaim.Value;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        try
        {
            var (user, departmentClaim, positionClaim) = await GetUserInfo();
            await _userManager.ReplaceClaimAsync(user, departmentClaim, new Claim(departmentClaim.Type, ViewModel.Department));
            await _userManager.ReplaceClaimAsync(user, positionClaim, new Claim(positionClaim.Type, ViewModel.Position));
        }
        catch (Exception exception)
        {
            ModelState.AddModelError(nameof(exception),$"Failed to update user profile. {exception.Message}");
        }

        SuccessMessage = "The user profile is saved successfully.";

        return Page();
    }

    private async Task<(IdentityUser, Claim, Claim)> GetUserInfo()
    {
        var user = await _userManager.FindByNameAsync(User.Identity!.Name);
        var claims = await _userManager.GetClaimsAsync(user);

        var departmentClaim = claims.FirstOrDefault(x => x.Type.Equals("Department"));
        var positionClaim = claims.FirstOrDefault(x => x.Type.Equals("Position"));

        return (user, departmentClaim, positionClaim);
    }
}

public class UserProfileViewModel
{
    public string Email { get; set; }

    [Required]
    public string Department { get; set; }

    [Required]
    public string Position { get; set; }
}