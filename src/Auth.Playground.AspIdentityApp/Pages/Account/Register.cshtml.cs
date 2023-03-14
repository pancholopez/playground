using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Auth.Playground.AspIdentityApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.Playground.AspIdentityApp.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmailService _emailService;

    [BindProperty]
    public RegisterViewModel ViewModel { get; set; }

    public RegisterModel(UserManager<IdentityUser> userManager, IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        var user = new IdentityUser
        {
            Email = ViewModel.Email,
            UserName = ViewModel.Email,
        };

        var result = await _userManager.CreateAsync(user, ViewModel.Password);

        if (result.Succeeded)
        {
            var claimDepartment = new Claim("Department", ViewModel.Department);
            var claimPosition = new Claim("Position", ViewModel.Position);

            await _userManager.AddClaimsAsync(user, new[] { claimDepartment, claimPosition });

            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink = Url.PageLink(
                pageName: "/Account/ConfirmEmail",
                values: new { userId = user.Id, token = confirmationToken });

            await _emailService.SendAsync(
                to: user.Email,
                from: "info@app.com",
                subject: "Confirm your email address",
                body: $"Please click on this link to confirm your email address : {confirmationLink}");

            return Redirect("/Account/Login");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("register", error.Description);
        }

        return Page();
    }
}

public class RegisterViewModel
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid Email address.")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }    //TODO: add password confirmation

    [Required]
    public string Department { get; set; }

    [Required]
    public string Position { get; set; }
}