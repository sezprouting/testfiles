// -----------------------------------------------------------------------
//
//      Created by Sez Flynn 2019
//      https://github.com/sezprouting/GithubApp-POC
//
//
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GitHubFunctionApp.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Octokit;
using Octokit.Internal;

namespace GitHubFunctionApp.Triggers
{
    public static class RepositoryHttpTriggers
    {
        private static long _installationId = 0; //TODO replace with values from authentication/client.
        
        public static SimpleJsonSerializer Serializer = new SimpleJsonSerializer();

        [FunctionName(nameof(RepositoryCollaboratorsEndpoint))]
        public static async Task<HttpResponseMessage> RepositoryCollaboratorsEndpoint(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "repositories/{repositoryId}/collaborators")]
            HttpRequestMessage request,
            long repositoryId,
            ILogger log)
        {
            if (request.Method == HttpMethod.Get)
            {
                log.LogInformation($"Fetching collaborators for repository id: {repositoryId}");
                IGitHubClient client = await GitHubClientService.GetClient(_installationId);
                var collaboratorClient = client.Repository.Collaborator;
                IReadOnlyList<User> collaborators;

                try
                {
                    collaborators = await collaboratorClient.GetAll(repositoryId);
                    List<string> collabIds = new List<string>();
                    if (collaborators != null)
                    {
                        collabIds.AddRange(collaborators.Select(x => x.Login));
                        log.LogInformation($"Repository {repositoryId} has the following collaborators: [{string.Join("; ", collabIds)}]");
                    }

                }
                catch (Exception e)
                {
                    log.LogError("Could not fetch collaborators:\n" + e);
                    return request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                return request.CreateResponse(HttpStatusCode.OK, collaborators);
            }

            return request.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
    }
}
