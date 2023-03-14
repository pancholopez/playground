using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Auth.Playground.AspIdentityApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Playground.AspIdentityApp.Pages.Account;

public class LoginAuthenticatorModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    [BindProperty] public AuthenticatorMfaViewModel ViewModel { get; set; } = new();

    public LoginAuthenticatorModel(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public void OnGet(bool rememberMe)
    {
        ViewModel.SecurityCode = string.Empty;
        ViewModel.RememberMe = rememberMe;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
            code: ViewModel.SecurityCode,
            isPersistent: ViewModel.RememberMe,
            rememberClient: false); //todo: offer user to remember this to not ask TFA for x days (checking back the cookie)

        if (result.Succeeded)
        {
            return RedirectToPage("/Index");
        }

        ModelState.AddModelError("login2FA",
            result.IsLockedOut
                ? "Your account is locked out. please wait 5 minutes."
                : "Failed to login.");

        return Page();
    }
}

public class AuthenticatorMfaViewModel
{
    [Required]
    [DisplayName("Security Code")]
    public string SecurityCode { get; set; }

    public bool RememberMe { get; set; }
}