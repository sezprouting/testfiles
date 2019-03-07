// -----------------------------------------------------------------------
//
//      Crated by Sez Flynn 2019
//      https://github.com/sezprouting/GithubApp-POC
//
// -----------------------------------------------------------------------

using System.Threading.Tasks;
using Octokit;

namespace GitHubFunctionApp.Services
{
    internal static class GitHubClientService
    {
        private static IGitHubClient _client;

        public static async Task<IGitHubClient> GetClient(long installationId)
        {
            if (_client == null)
            {
                string installationToken = await GithubAppTokenService.GetTokenForInstallationAsync(installationId);
                var userAgent = new ProductHeaderValue("SezTestApp");
                _client = new GitHubClient(userAgent)
                {
                    Credentials = new Credentials(installationToken)
                };
            }

            return _client;
        }
    }
}
