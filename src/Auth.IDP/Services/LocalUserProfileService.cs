using System.Security.Claims;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

namespace Auth.IDP.Services;

public class LocalUserProfileService : IProfileService
{
    private readonly ILocalUserService _localUserService;

    public LocalUserProfileService(ILocalUserService localUserService)
    {
        _localUserService = localUserService;
    }
    
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var subjectId = context.Subject.GetSubjectId();
        var claimsForUser = (await _localUserService.GetUserClaimsBySubjectAsync(subjectId)).ToList();
        context.AddRequestedClaims(claimsForUser.Select(x => new Claim(x.Type, x.Value)).ToList());
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = true;
        await Task.CompletedTask;
        // var subjectId = context.Subject.GetSubjectId();
        // context.IsActive = await _localUserService.IsUserActive(subjectId);
    }
}