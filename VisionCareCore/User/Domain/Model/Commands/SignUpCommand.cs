using VisionCareCore.User.Domain.Model.Aggregates;

namespace VisionCareCore.User.Domain.Model.Commands;

public record SignUpCommand(
    string Email,
    string Password,
    string Name,
    string LastName,
    DateTime Birthday,              
    DateTime DateCreatedAt,
    VisualImpairmentLevel VisualImpairment
);