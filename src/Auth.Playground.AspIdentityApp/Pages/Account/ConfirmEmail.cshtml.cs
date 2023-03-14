using Auth.Playground.AspIdentityApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Playground.AspIdentityApp.Pages.Account;

public class ConfirmEmailModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    [BindProperty]
    public string Message { get; set; }

    public ConfirmEmailModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGetAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user != null)
        {
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                Message = "Email address is successfully confirmed. You can login now.";
            }
        }
        else
        {
            Message = "Failed to validate email.";
        }
        return Page();
    }
}