using System.Security.Claims;

namespace Gateway.Root.Shared;

public static class HttpContextJwtUtils
{
    public static Guid GetUserId(this HttpContext context)
    {
        var id = context.User.FindFirst(claim => claim.Type == ClaimTypes.PrimarySid)?.Value;

        if (id == null)
            throw new ClaimIsNotSetException(ClaimTypes.PrimarySid);

        return Guid.Parse(id);
    }
}
