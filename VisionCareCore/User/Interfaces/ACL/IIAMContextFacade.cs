using VisionCareCore.User.Domain.Model.Aggregates;

namespace VisionCareCore.User.Interfaces.ACL;

public interface IIamContextFacade
{
    Task<Guid> CreateAuthUser(
        string email,
        string password,
        string name,
        string lastname,
        DateTime birthday,
        DateTime dateCreatedAt,
        VisualImpairmentLevel visualImpairment);    Task<Guid> FetchAuthUserIdByEmail(string email);
    Task<string> FetchAuthUsernameByUserId(Guid userId);
}