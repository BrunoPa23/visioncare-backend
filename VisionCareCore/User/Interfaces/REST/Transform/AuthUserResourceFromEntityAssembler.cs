using VisionCareCore.User.Interfaces.REST.Resources;

namespace VisionCareCore.User.Interfaces.REST.Transform;

public static class AuthUserResourceFromEntityAssembler
{
    public static AuthUserResource ToResourceFromEntity(Domain.Model.Aggregates.AuthUser user)
    {
        return new AuthUserResource(
            user.Id,
            user.Email,
            user.Name,
            user.LastName,
            user.Birthday,
            user.VisualImpairment.ToString() );
    }
}