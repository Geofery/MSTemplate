using Internal;
using System.Threading.Tasks;
using NServiceBus;
using SharedMessages;

namespace Service3.Web.Handlers
{
    public class MessageHandler : IHandleMessages<MessageFromService2>
    {
        public Task Handle(MessageFromService2 message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Service3 received message: {message.Content}");
            return Task.CompletedTask;
        }
    }
}
