using Application.Commands;
using Application.Events;
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
        var isValid = await _userRepository.ValidateUserAsync(message.UserId);
        if (isValid)
        {
            await context.Publish(new UserValidated { UserId = message.UserId });
        }
        else
        {
            await context.Publish(new UserValidationFailed
            {
                UserId = message.UserId,
                Reason = "Invalid user or address."
            });
        }
    }
}
}