using VisionCareCore.User.Domain.Model.Aggregates;
using VisionCareCore.User.Domain.Model.Commands;

namespace VisionCareCore.User.Domain.Services;

public interface IAuthUserCommandService
{
    /**
        * <summary>
        *     Handle sign in command
        * </summary>
        * <param name="command">The sign in command</param>
        * <returns>The authenticated user and the JWT token</returns>
        */
    Task<(AuthUser authUser, string token)> Handle(SignInCommand command);

    /**
        * <summary>
        *     Handle sign up command
        * </summary>
        * <param name="command">The sign up command</param>
        * <returns>A confirmation message on successful creation.</returns>
        */
    Task Handle(SignUpCommand command);
}