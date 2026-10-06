using VisionCareCore.User.Interfaces.REST.Resources;

namespace VisionCareCore.User.Interfaces.REST.Transform;

public static class AuthenticatedUserResourceFromEntityAssembler
{
    public static AuthenticatedUserResource ToResourceFromEntity(
        Domain.Model.Aggregates.AuthUser user, string token)
    {
        return new AuthenticatedUserResource(user.Id, user.Email,user.Name, token);
    }
}