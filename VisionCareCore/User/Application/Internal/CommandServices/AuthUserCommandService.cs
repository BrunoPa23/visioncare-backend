
using VisionCareCore.Shared.Domain.Repositories;
using VisionCareCore.User.Application.Internal.OutboundServices;
using VisionCareCore.User.Domain.Model.Aggregates;
using VisionCareCore.User.Domain.Model.Commands;
using VisionCareCore.User.Domain.Repositories;
using VisionCareCore.User.Domain.Services;

namespace VisionCareCore.User.Application.Internal.CommandServices;

public class AuthUserCommandService(
    IAuthUserRepository userRepository,
    ITokenService tokenService,
    IHashingService hashingService,
    IUnitOfWork unitOfWork)
    : IAuthUserCommandService
{
    /**
     * <summary>
     *     Handle sign in command
     * </summary>
     * <param name="command">The sign in command</param>
     * <returns>The authenticated user and the JWT token</returns>
     */
    public async Task<(AuthUser authUser, string token)> Handle(SignInCommand command)
    {
        var user = await userRepository.FindByEmailAsync(command.Email);

        if (user == null || !hashingService.VerifyPassword(command.Password, user.PasswordHash))
            throw new Exception("Invalid username or password");

        var token = tokenService.GenerateToken(user);

        return (user, token);
    }

   

    /**
     * <summary>
     *     Handle sign up command
     * </summary>
     * <param name="command">The sign up command</param>
     * <returns>A confirmation message on successful creation.</returns>
     */
    public async Task Handle(SignUpCommand command)
    {
        if (userRepository.ExistsByEmail(command.Email))
            throw new Exception($"Email {command.Email} is already taken");

        var hashedPassword = hashingService.HashPassword(command.Password);
        var user = new AuthUser(
            command.Email,
            hashedPassword,
            command.Name,
            command.LastName,
            command.Birthday,
            command.DateCreatedAt,
            command.VisualImpairment);        try
        {
            await userRepository.AddAsync(user);
            await unitOfWork.CompleteAsync();
        }
        catch (Exception e)
        {
            throw new Exception($"An error occurred while creating user: {e.Message}");
        }
    }
}