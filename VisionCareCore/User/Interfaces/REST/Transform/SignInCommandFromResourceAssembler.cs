using VisionCareCore.User.Domain.Model.Commands;
using VisionCareCore.User.Interfaces.REST.Resources;

namespace VisionCareCore.User.Interfaces.REST.Transform;

public static class SignInCommandFromResourceAssembler
{
    public static SignInCommand ToCommandFromResource(SignInResource resource)
    {
        return new SignInCommand(resource.Email, resource.Password);
    }
}