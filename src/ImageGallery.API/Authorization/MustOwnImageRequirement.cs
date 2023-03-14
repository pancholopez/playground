using ImageGallery.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace ImageGallery.API.Authorization;

public class MustOwnImageRequirement:IAuthorizationRequirement
{
    public MustOwnImageRequirement()
    {
        
    }
}

public class MustOwnImageHandler : AuthorizationHandler<MustOwnImageRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IGalleryRepository _repository;

    public MustOwnImageHandler(IHttpContextAccessor httpContextAccessor, IGalleryRepository repository)
    {
        _httpContextAccessor = httpContextAccessor;
        _repository = repository;
    }
    
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        MustOwnImageRequirement requirement)
    {
        var imageId = _httpContextAccessor.HttpContext?.GetRouteValue("id")?.ToString();

        if (!Guid.TryParse(imageId, out var imageIdGuid))
        {
            context.Fail();
            return;
        }

        var ownerId = context.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
        if (ownerId == null)
        {
            context.Fail();
            return;
        }

        if (!await _repository.IsImageOwnerAsync(imageIdGuid,ownerId))
        {
            context.Fail();
            return;
        }
        
        context.Succeed(requirement);
    }
}