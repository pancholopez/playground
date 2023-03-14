using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Playground.AspIdentityApp.Pages.Account;

public class PasswordReset : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    [BindProperty] public PasswordResetModel ViewModel { get; set; } = new();

    public PasswordReset(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }
    
    public void OnGet(string token, string userId)
    {
        ViewModel.Token = token;
        ViewModel.UserId = userId;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(ViewModel.UserId);
            if (user!=null)
            {
                var result = await _userManager.ResetPasswordAsync(user, ViewModel.Token, ViewModel.Password);
                if (result.Succeeded)
                {
                    return Redirect("/Account/Login");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("PasswordReset", error.Description);
                    }
                }
            }
            ModelState.AddModelError("PasswordReset", "Invalid Request");
        }

        return Page();
    }
}

public class PasswordResetModel
{
    public string Token { get; set; }

    public string UserId { get; set; } //TODO: change this for email maybe? to not show it in hidden field
    
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Compare("Password")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}