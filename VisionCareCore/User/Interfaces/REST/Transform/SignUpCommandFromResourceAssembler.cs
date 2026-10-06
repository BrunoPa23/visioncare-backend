using VisionCareCore.User.Domain.Model.Commands;
using VisionCareCore.User.Interfaces.REST.Resources;
using VisionCareCore.User.Domain.Model.Aggregates;

namespace VisionCareCore.User.Interfaces.REST.Transform;

public static class SignUpCommandFromResourceAssembler
{
    public static SignUpCommand? ToCommand(SignUpResource resource)
    {
        if (!Enum.TryParse<VisualImpairmentLevel>(
                resource.VisualImpairment,
                ignoreCase: true,
                out var parsedLevel))
        {
            return null; // Nivel inválido
        }

        return new SignUpCommand(
            resource.Email,
            resource.Password,
            resource.Name,
            resource.LastName,
            resource.Birthday,
            DateTime.UtcNow,
            parsedLevel
        );
    }
}