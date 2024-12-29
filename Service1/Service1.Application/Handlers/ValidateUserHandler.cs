using System;
using System.Threading.Tasks;
using SharedMessages;
using Domain.Repositories;
using NServiceBus;
using Microsoft.Extensions.Logging;

namespace Application.Handlers
{
    public class ValidateUserHandler : IHandleMessages<ValidateUser>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ValidateUserHandler> _logger;

        public ValidateUserHandler(IUserRepository userRepository, ILogger<ValidateUserHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ValidateUser message, IMessageHandlerContext context)
        {
            _logger.LogInformation("Handling ValidateUser command. Email: {Email}", message.Email);

            try
            {
                var userId = await _userRepository.ValidateUserAsync(message.Email);

                if (userId != Guid.Empty)
                {
                    _logger.LogInformation("User validated successfully. Email: {Email}, UserId: {UserId}", message.Email, userId);

                    await context.Publish(new UserValidated
                    {
                        UserId = userId,
                        OrderId = message.OrderId
                    });
                }
                else
                {
                    _logger.LogWarning("User validation failed. Email: {Email}", message.Email);

                    await context.Publish(new UserValidationFailed
                    {
                        UserId = Guid.Empty,
                        OrderId = message.OrderId,
                        Reason = "User not in system"
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while validating user. Email: {Email}", message.Email);
                throw;
            }
        }
    }
}
