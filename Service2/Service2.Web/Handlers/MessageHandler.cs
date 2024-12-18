using NServiceBus;
using SharedMessages;
using System.Threading.Tasks;

namespace Service2.Web.Handlers
{
    public class MessageHandler : IHandleMessages<MessageFromService1>
    {
        public async Task Handle(MessageFromService1 message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Service2 received message: {message.Content}");

            var messageToService3 = new MessageFromService2
            {
                Content = "Hello from Service2!"
            };

            await context.Publish(messageToService3);
        }
    }
}
