using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MailChimp.Net.Interfaces;
using MailChimp.Models;
using MailChimp.Net.Models;
using System.Linq;

namespace MailChimp
{
    public class UpdateUserTags
    {
        private readonly IMailChimpManager _mailChimpManager;

        public UpdateUserTags(IMailChimpManager mailChimpManager)
        {
            _mailChimpManager = mailChimpManager;
        }

        /// <summary>
        /// Update tags for user
        /// </summary>
        /// <param name="req"><see cref="AddUserTagsModel"/></param>
        /// <param name="listId">Id of audience for user.</param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("AddUserTag")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UpdateUserTags/{listId}")] AddUserTagsModel req,
            string listId,
            ILogger log)
        {
            var tags = new Tags()
            {
                MemberTags = req.Tags.ToList()
            };

            await _mailChimpManager.Members.AddTagsAsync(listId, req.UserEmail, tags);

            return new OkObjectResult($"Tags updated for user: {req.UserEmail}");
        }
    }
}
