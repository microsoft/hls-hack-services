using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Options;

namespace Registration.Api.Services
{
    public class EventGridMessageService : IMessageService
    {
        private readonly EventGridPublisherClient _messageClient;

        public EventGridMessageService(IOptions<EventGridMessageServiceConfiguration> config)
        {
            _messageClient = new EventGridPublisherClient(new Uri(config.Value.Endpoint), new Azure.AzureKeyCredential(config.Value.AccessKey));
        }

        public async Task SendMessageAsync(Message message)
        {
            var evt = new EventGridEvent(message.Subject, message.EventType, "1.0", message.Data);
            await _messageClient.SendEventAsync(evt);
        }
    }

    public class EventGridMessageServiceConfiguration
    {
        public string Endpoint { get; set; }
        public string AccessKey { get; set; }
    }
}
