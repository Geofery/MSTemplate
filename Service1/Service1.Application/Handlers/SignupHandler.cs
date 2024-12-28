using System;
using System.Threading.Tasks;
using NServiceBus;
using Domain.Models;
using Domain.Repositories;
using SharedMessages;
using Microsoft.Extensions.Logging;
using NServiceBus.Transport;

namespace Application.Handlers
{
    public class SignupHandler : IHandleMessages<SignupCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<SignupHandler> _logger;

        public SignupHandler(IUserRepository userRepository, ILogger<SignupHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(SignupCommand message, IMessageHandlerContext context)
        {
            _logger.LogInformation("Handling SignupCommand. Email: {Email}, Name: {Name}, OrderId: {OrderId}",
                message.Email, message.Name, message.OrderId);

            try
            {
                var user = CreateUser(message);
                var savedUser = await _userRepository.SaveMemberAsync(user);

                if (savedUser == null)
                {
                    _logger.LogWarning("Failed to save user. Email: {Email}, Name: {Name}, OrderId: {OrderId}",
                        message.Email, message.Name, message.OrderId);
                    return;
                }

                _logger.LogInformation("User saved successfully. UserId: {UserId}, Email: {Email}, Name: {Name}",
                    savedUser.UserId, savedUser.Email, savedUser.Name);

                var signupCompletedEvent = CreateSignupCompletedEvent(savedUser, message.OrderId);
                await context.Publish(signupCompletedEvent);

                _logger.LogInformation("SignupCompleted event published successfully. UserId: {UserId}, OrderId: {OrderId}",
                    savedUser.UserId, message.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while signing up user. Email: {Email}, Name: {Name}, OrderId: {OrderId}",
                    message.Email, message.Name, message.OrderId);
                throw;
            }
        }

        private User CreateUser(SignupCommand message)
        {
            return new User(
                Guid.NewGuid(),
                message.Name,
                message.Email,
                message.Password,
                new Address(
                    Guid.NewGuid(),
                    message.Street,
                    message.City,
                    message.PostalCode
                )
            );
        }

        private SignupCompleted CreateSignupCompletedEvent(User savedUser, Guid orderId)
        {
            return new SignupCompleted(
                savedUser.UserId,
                savedUser.Name,
                savedUser.Email,
            savedUser.Password,
            savedUser.Address.Id,
            savedUser.Address.Street,
            savedUser.Address.City,
            savedUser.Address.PostalCode,
            orderId
            );
        }
    }
}
