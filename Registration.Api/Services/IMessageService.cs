using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Registration.Api.Services
{
    public interface IMessageService
    {
        Task SendMessageAsync(Message message);
    }

    public record Message
    {
        public string Subject { get; set; }
        public string EventType { get; set; }
        public object Data { get; set; }
    }
}
