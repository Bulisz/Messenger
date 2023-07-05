using System.Security.Claims;

namespace Messenger.Backend.MiddlewareConfig;

public static class ClaimsPrincipalExtensions
{
    public static string GetCurrentUserId(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);
        return user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Current user Id is null");
    }
}
