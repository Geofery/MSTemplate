using System;
using NServiceBus;
using SharedMessages;

namespace Service1.Web
{
	public class SignupHandler : IHandleMessages<SignupDTO>
	{
        //private readonly ILogger<SignupHandler> _logger;

        /*public SignupHandler(ILogger<SignupHandler> logger) //TODO Implement repo  
		{
            _logger = logger;
		}*/

        public async Task Handle(SignupDTO message, IMessageHandlerContext context)
        {
            //_logger.LogInformation("Received command signup new member: {email}", message.Email);
            //await _userRepository.UpdateUser(message).ConfigureAwait(false);
            Console.WriteLine($"MESSAGE IN HANDLER {message}");
            //await context.Publish(message).ConfigureAwait(false);
        }
    }
}

