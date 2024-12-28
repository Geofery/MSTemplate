using System;
using NServiceBus;
using Domain.Models;
using Domain.Repositories;
using SharedMessages;
using Microsoft.Extensions.Logging;

namespace Application.Handlers
{ 
	public class SignupHandler : IHandleMessages<SignupCommand>
	{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SignupHandler> _logger;

    public SignupHandler(IUserRepository userRepository, ILogger<SignupHandler> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger;
    }

    public async Task Handle(SignupCommand message, IMessageHandlerContext context)
    {
        var signup = new User(new Guid(), message.Name, message.Email, message.Password,
            new Address(new Guid(), message.Street, message.City, message.PostalCode));
        var result = await _userRepository.SaveMemberAsync(signup).ConfigureAwait(false);
            
        var signupCompleted = new SignupCompleted(result.UserId, result.Name, result.Email,
            result.Password, result.Address.Id, result.Address.Street, result.Address.City, result.Address.PostalCode, message.OrderId);
        _logger.LogInformation($"Message published from UserManagement{message}");
        await context.Publish(signupCompleted).ConfigureAwait(false);
    }
}
}