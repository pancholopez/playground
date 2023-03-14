using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Playground.AspIdentityApp.Pages.Account;

public class ForgotPassword : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    [BindProperty] public ForgotPasswordModel ViewModel { get; set; } = new();

    public ForgotPassword(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }
    
    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(ViewModel.Email);
            if (user!=null)
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var confirmationUrl = Url.PageLink(
                    pageName: "/Account/PasswordReset",
                    values: new { userId = user.Id, token = resetToken });
                
                //Todo: implement again in memory email send
                await System.IO.File.WriteAllTextAsync("resetLink.txt", confirmationUrl);
            }
            else
            {
                //TODO: email user and inform they dont have an account
            }
            return Redirect("/Account/Login");
        }

        return Page();
    }
}

public class ForgotPasswordModel
{
    [Required] [EmailAddress] public string Email { get; set; }
}