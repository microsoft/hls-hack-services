using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using MailChimp.Models;

namespace MailChimp
{
    public class SubscribeUser
    {
        private readonly IMailChimpManager _mailChimpManager;
        public SubscribeUser(IMailChimpManager mailChimpManager)
        {
            _mailChimpManager = mailChimpManager;
        }

        /// <summary>
        /// Subscribes user to mailing list
        /// </summary>
        /// <param name="user"><see cref="MailChimp.Models.SubscribeUserModel"/></param>
        /// <param name="log"></param>
        /// <returns>200</returns>
        [FunctionName("SubscribeUser")]
        public async Task<IActionResult> SubscribeUserPost(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "SubscribeUser/{listId}")] SubscribeUserModel user,
            string listId,
            ILogger log)
        {          
            if (user == null)
            {
                return new BadRequestResult();
            }

            // Use the Status property if updating an existing member
            var member = new Member { EmailAddress = user.UserEmail, StatusIfNew = Status.Subscribed, Status = Status.Subscribed };
            member.MergeFields.Add("FNAME", user.FirstName);
            member.MergeFields.Add("LNAME", user.LastName);
            await _mailChimpManager.Members.AddOrUpdateAsync(listId, member);

            var responseMessage = $"User with email: {user.UserEmail} added to subscription with id: {listId}";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("UnsubscribeUser")]
        public async Task<IActionResult> UnsubscribeUserPost(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UnsubscribeUser/{listId}")] UnsubscribeUserModel user,
            string listId,
            ILogger log)
        {
            var userToUnsubscribe = await _mailChimpManager.Members.GetAsync(listId, user.UserEmail);
            if (userToUnsubscribe != null)
            {
                userToUnsubscribe.Status = Status.Unsubscribed;
                await _mailChimpManager.Members.AddOrUpdateAsync(listId, userToUnsubscribe);
            }

            return new NoContentResult();
        }
    }
}
