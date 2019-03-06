using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace GitHubFunctionApp
{
    public static class GithubHttpTrigger
    {
        [FunctionName(nameof(GithubWebhook))]
        public static HttpResponseMessage GithubWebhook(
            [HttpTrigger("POST", WebHookType = "github")]
            HttpRequestMessage req,
            TraceWriter log)
        {
            log.Info("GithubWebhook has been triggered.");
            return req.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
