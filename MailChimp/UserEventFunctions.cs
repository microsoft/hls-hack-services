// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using MailChimp.Net.Interfaces;
using Newtonsoft.Json;
using MailChimp.Models;
using MailChimp.Net.Models;
using System.Threading.Tasks;
using Mailchimp.Services;
using System.Linq;

namespace MailChimp
{
    public class UserEventFunctions
    {

        private readonly IMailChimpManager _mailChimpManager;
        private readonly string _listId;
        private readonly RegistrationService _registrationService;

        public UserEventFunctions(IMailChimpManager mailChimpManager, RegistrationService registrationService)
        {
            _mailChimpManager = mailChimpManager;
            _registrationService = registrationService;
            _listId = Environment.GetEnvironmentVariable("ListId");
        }

        [FunctionName("UserAdded")]
        public async Task UserAdded([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            try
            {
                if (!eventGridEvent.EventType.Equals("User.Added", StringComparison.InvariantCultureIgnoreCase))
                    return;

                var eventData = ParseEventData(eventGridEvent.Data);
                var registrationUser = (await _registrationService.FindByEmailAsync(eventData.Email)).FirstOrDefault();

                var member = new Member { EmailAddress = eventData.Email, StatusIfNew = Status.Subscribed, Status = Status.Subscribed };
                member.MergeFields.Add("FNAME", registrationUser.FirstName);
                member.MergeFields.Add("LNAME", registrationUser.LastName);

                await _mailChimpManager.Members.AddOrUpdateAsync(_listId, member);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error processing 'User.Added' event");
                throw ex;
            }
        }

        [FunctionName("UserUpdated")]
        public async Task UserUpdated([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            try
            {
                if (!eventGridEvent.EventType.Equals("User.Updated", StringComparison.InvariantCultureIgnoreCase))
                    return;

                var eventData = ParseEventData(eventGridEvent.Data);
                var registrationUser = (await _registrationService.FindByEmailAsync(eventData.Email)).FirstOrDefault();

                var status = (bool)registrationUser.Active ? Status.Subscribed : Status.Unsubscribed;

                var member = new Member { EmailAddress = eventData.Email, Status = status };

                await _mailChimpManager.Members.AddOrUpdateAsync(_listId, member);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error processing 'User.Updated' event");
                throw ex;
            }
        }

        [FunctionName("UserRemoved")]
        public async Task UserRemoved([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            try
            {
                if (!eventGridEvent.EventType.Equals("User.Removed", StringComparison.InvariantCultureIgnoreCase))
                    return;

                var eventData = ParseEventData(eventGridEvent.Data);
                var member = new Member { EmailAddress = eventData.Email, Status = Status.Unsubscribed };
                await _mailChimpManager.Members.AddOrUpdateAsync(_listId, member);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error processing 'User.Removed' event");
            }
        }

        private UserEventData ParseEventData(object eventData)
        {
            var jsonObj = JsonConvert.SerializeObject(eventData);
            return JsonConvert.DeserializeObject<UserEventData>(jsonObj);
        }

    }
}
