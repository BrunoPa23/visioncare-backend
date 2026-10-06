using VisionCareCore.User.Domain.Model.Aggregates;
using VisionCareCore.User.Domain.Model.Commands;
using VisionCareCore.User.Domain.Model.Queries;
using VisionCareCore.User.Domain.Services;

namespace VisionCareCore.User.Interfaces.ACL.Services;

public class IamContextFacade(IAuthUserCommandService userCommandService, IAuthUserQueryService userQueryService) : IIamContextFacade
{
    public async Task<Guid> CreateAuthUser(
        string email, string password, string name,
        string lastname, DateTime birthday, DateTime dateCreated, VisualImpairmentLevel visualImpairment)
    {
        var signUpCommand = new SignUpCommand(
            email,
            password,
            name,
            lastname,
            birthday,
            dateCreated,
            visualImpairment);

        await userCommandService.Handle(signUpCommand);

        var getUserByUsernameQuery = new GetAuthUserByEmailQuery(email);
        var result = await userQueryService.Handle(getUserByUsernameQuery);
    
        return result?.Id ?? Guid.Empty;
    }


    public async Task<Guid> FetchAuthUserIdByEmail(string email)
    {
        var getAuthUserByUsernameQuery = new GetAuthUserByEmailQuery(email);
        var result = await userQueryService.Handle(getAuthUserByUsernameQuery);
        return result?.Id ?? Guid.Empty;
    }

    public async Task<string> FetchAuthUsernameByUserId(Guid userId)
    {
        var getAuthUserByIdQuery = new GetAuthUserByIdQuery(userId);
        var result = await userQueryService.Handle(getAuthUserByIdQuery);
        return result?.Email ?? string.Empty;
    }
}