namespace VisionCareCore.User.Interfaces.REST.Resources;

public record AuthUserResource(Guid Id, string Email,string Name,string LastName, DateTime Birthday, string VisualImpairment );