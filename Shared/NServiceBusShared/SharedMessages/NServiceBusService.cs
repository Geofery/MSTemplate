using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace SharedMessages
{
    public class NServiceBusService : IHostedService
    {
        public IMessageSession _messageSession { get; private set; }
        private IEndpointInstance _endpointInstance;
        private readonly string _endpointName;

        public NServiceBusService(string endpointName)
        {
            _endpointName = endpointName;
        }

        public IMessageSession MessageSession
        {
            get
            {
                if (_messageSession == null)
                {
                    throw new InvalidOperationException("The NServiceBus MessageSession is not yet initialized.");
                }
                return _messageSession;
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting NServiceBus Endpoint...");

            var endpointConfiguration = new EndpointConfiguration(_endpointName);
            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            transport.StorageDirectory($"../Build/NServiceBusTransport/{_endpointName}");

            endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            _endpointInstance = await Endpoint.Start(endpointConfiguration, cancellationToken);
            Console.WriteLine(_endpointInstance.ToString());
            _messageSession = _endpointInstance;

            Console.WriteLine("NServiceBus Endpoint started successfully.");
        }


        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_endpointInstance != null)
            {
                await _endpointInstance.Stop();
            }
        }
    }
}

