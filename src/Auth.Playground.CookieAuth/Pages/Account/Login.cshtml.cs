using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Auth.Playground.CookieAuth.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace Auth.Playground.CookieAuth.Pages.Account;

public class LoginModel : PageModel
{
    [BindProperty]
    public Credential Credential { get; set; }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        if (Credential.UserName.Equals("admin") && Credential.Password.Equals("password"))
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, "Admin"),
                new(ClaimTypes.Email, "Admin@mail.com"),
                new("Department", "HR"),
                new("Admin", "true"),
                new("HRManager","true"),
                new("HiringDate","2022-01-01")
            };
            var identity = new ClaimsIdentity(claims, Constants.AuthenticationSchema);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = Credential.RememberMe
            };

            await HttpContext.SignInAsync(Constants.AuthenticationSchema, claimsPrincipal, authProperties);

            return RedirectToPage("/index");
        }

        return Page();
    }
}

public class Credential
{
    [Required]
    [DisplayName("User Name")]
    public string UserName { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DisplayName("Remember Me")]
    public bool RememberMe { get; set; }
}