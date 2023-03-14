using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace Auth.Playground.AspIdentityApp.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;

    [BindProperty]
    public Credential Credential { get; set; }

    public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogInformation("Loading User Login screen");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var timer = new Stopwatch();
        timer.Start();
        
        var result = await _signInManager.PasswordSignInAsync(
            userName: Credential.Email,
            password: Credential.Password,
            isPersistent: Credential.RememberMe,
            lockoutOnFailure: true);

        timer.Stop();
        _logger.LogDebug("Query Signin User for {Email} finished in {Milliseconds} milliseconds", 
            Credential.Email, timer.ElapsedMilliseconds);
        
        _logger.LogInformation("Logging attempt by {Email}", Credential.Email);
        if (result.Succeeded)
        {
            return RedirectToPage("/Index");
        }
        
        if (result.RequiresTwoFactor)
        {
            // for email code 2FA
            //return RedirectToPage("/Account/LoginTwoFactor", new
            //{
            //    Credential.Email, 
            //    Credential.RememberMe
            //});
            return RedirectToPage("/Account/LoginAuthenticator", new
            {
                Credential.RememberMe
            });
        }
        
        if (result.IsLockedOut)
        {
            //send email account got locked
        }
        
        ModelState.AddModelError("login",
            result.IsLockedOut
                ? "Your account is locked out. please wait 10 minutes."
                : "Invalid email or password.");

        return Page();
    }
}

public class Credential
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DisplayName("Remember Me")]
    public bool RememberMe { get; set; }
}