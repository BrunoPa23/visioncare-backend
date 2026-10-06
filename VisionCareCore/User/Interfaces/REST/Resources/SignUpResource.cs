using VisionCareCore.User.Domain.Model.Aggregates;

namespace VisionCareCore.User.Interfaces.REST.Resources;

public record SignUpResource(
    string Email,
    string Password,
    string Name,
    string LastName,
    DateTime Birthday,
    string VisualImpairment
);