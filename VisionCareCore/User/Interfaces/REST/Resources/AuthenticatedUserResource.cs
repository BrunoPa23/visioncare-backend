namespace VisionCareCore.User.Interfaces.REST.Resources;

public record AuthenticatedUserResource(Guid Id, string Email,string Name, string Token);