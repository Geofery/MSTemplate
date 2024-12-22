using NServiceBus;
using SharedMessages;
using System.Threading.Tasks;

namespace Web.Handlers
{
    public class MessageHandler : IHandleMessages<SignupCompleted>
    {
        public async Task Handle(SignupCompleted message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Service2 received message: {message.Name}");

            var messageToService3 = new MessageFromService2
            {
                Content = "Hello from Service2!"
            };

            await context.Publish(messageToService3);
        }
    }
}
