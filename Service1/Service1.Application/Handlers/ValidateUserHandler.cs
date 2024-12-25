using SharedMessages;
using Domain.Repositories;
using NServiceBus;


namespace Application.Handlers
{ 
public class ValidateUserHandler : IHandleMessages<ValidateUser>
{
    private readonly IUserRepository _userRepository;

    public ValidateUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(ValidateUser message, IMessageHandlerContext context)
    {
        var isValid = await _userRepository.ValidateUserAsync(message.Email);
        if (isValid != Guid.Empty)
        {
            await context.Publish(new UserValidated { UserId = isValid });
        }
        else
        {
            await context.Publish(new UserValidationFailed
            {
                UserId = isValid,
                Reason = "User not in system"
            });
        }
    }
}
}