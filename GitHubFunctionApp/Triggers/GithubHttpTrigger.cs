// -----------------------------------------------------------------------
//
//      Crated by Sez Flynn 2019
//      https://github.com/sezprouting/GithubApp-POC
//
// -----------------------------------------------------------------------
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GitHubFunctionApp.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Octokit;
using Octokit.Internal;

namespace GitHubFunctionApp.Triggers
{
    public static class GithubHttpTrigger
    {
        [FunctionName(nameof(GithubWebhook))]
        public static async Task<HttpResponseMessage> GithubWebhook(
            [HttpTrigger("POST", WebHookType = "github")]
            HttpRequestMessage req,
            TraceWriter log)
        {

            log.Info("GithubWebhook has been triggered. Request content follows:");
            string json = await req.Content.ReadAsStringAsync();
            log.Info(json);

            var serializer = new SimpleJsonSerializer();
            ActivityPayload payload = serializer.Deserialize<ActivityPayload>(json);

            IGitHubClient client = await GitHubClientService.GetClient(payload.Installation.Id);
            var collaboratorClient = client.Repository.Collaborator;
            try
            {
                var collaborators = await collaboratorClient.GetAll(payload.Repository.Id);
                log.Info($"Collaborators on this repository are:\n{serializer.Serialize(collaborators)}");
            }
            catch (Exception e)
            {
                log.Error("Could not fetch collaborators:\n" +  e);
            }


            return req.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
