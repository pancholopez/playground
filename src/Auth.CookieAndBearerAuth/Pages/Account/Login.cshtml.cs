using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Auth.CookieAndBearerAuth.Pages.Account;

public class Login : PageModel
{
    [BindProperty]
    public Credential Credential { get; set; } = new();
    
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
                new(ClaimTypes.Name, "John Doe"),
                new(ClaimTypes.Email, "admin@mail.com"),
                new("Department", "HR"),
                new("Admin", "true"),
                new("HRManager","true"),
                new("HiringDate","2022-01-01")
            };
            var identity = new ClaimsIdentity(claims, "Cookies");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = Credential.RememberMe
            };

            await HttpContext.SignInAsync("Cookies", claimsPrincipal, authProperties);

            return RedirectToPage("/index");
        }

        return Page();
    }
}

public class Credential
{
    [Required]
    [DisplayName("User Name")]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }  = string.Empty;

    [DisplayName("Remember Me")]
    public bool RememberMe { get; set; }
}