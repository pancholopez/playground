namespace Auth.Playground.AspIdentityApp.Services;
public class InMemoryEmailService : IEmailService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public InMemoryEmailService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task SendAsync(string to, string from, string subject, string body)
    {
        var match = body.Split(" : ");
        _httpContextAccessor.HttpContext.Session.SetString("emailBody", match[1]);
        await Task.CompletedTask;
    }
}