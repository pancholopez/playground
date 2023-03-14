using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Auth.Playground.AspIdentityApp.Data;
using Auth.Playground.AspIdentityApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Playground.AspIdentityApp.Pages.Account;

public class LoginTwoFactor : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly SignInManager<IdentityUser> _signInManager;

    [BindProperty] public EmailMfaViewModel ViewModel { get; set; } = new();

    public LoginTwoFactor(
        UserManager<IdentityUser> userManager, 
        IEmailService emailService,
        SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _emailService = emailService;
        _signInManager = signInManager;
    }

    public async Task OnGetAsync(string email, bool rememberMe)
    {
        ViewModel.SecurityCode = string.Empty;
        ViewModel.RememberMe = rememberMe;
        
        var user = await _userManager.FindByEmailAsync(email);
        var securityCode = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);

        await _emailService.SendAsync(
            to: email,
            from: "noreply@mywebapp.com",
            subject: "My Web app OTP",
            body: $"Use this code as OTP: {securityCode}");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        var result = await _signInManager.TwoFactorSignInAsync(
            provider: TokenOptions.DefaultEmailProvider,
            code: ViewModel.SecurityCode,
            isPersistent: ViewModel.RememberMe,
            rememberClient: false);
            
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

public class EmailMfaViewModel
{
    [Required]
    [DisplayName("Security Code")]
    public string SecurityCode { get; set; }
    public bool RememberMe { get; set; }
}