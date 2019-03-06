// -----------------------------------------------------------------------
//
//      Created by Thomas Levesque
//      Copied from: https://github.com/thomaslevesque/DontMergeMeYet 
//
//
// -----------------------------------------------------------------------
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Helpers;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace GitHubFunctionApp.Services
{
    internal class GithubAppTokenService
    {
        public static Task<string> GetTokenForApplicationAsync()
        {
            //    // var settings = _settingsProvider.Settings;

            var key = new RsaSecurityKey(CryptoHelper.GetRsaParameters("replace me from settings later"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(claims: new[]
                {
                        new Claim("iat", now.ToUnixTimeStamp().ToString(), ClaimValueTypes.Integer),
                        new Claim("exp", now.AddMinutes(10).ToUnixTimeStamp().ToString(), ClaimValueTypes.Integer),
                        // replace app id with settings call
                        new Claim("iss", "replace me with app id")
                    },
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Task.FromResult(jwt);
        }

        public async Task<string> GetTokenForInstallationAsync(int installationId)
        {
            string appToken = await GetTokenForApplicationAsync();
            using (var client = new HttpClient())
            {
                string url = $"https://api.github.com/installations/{installationId}/access_tokens";
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Headers =
                    {
                        Authorization = new AuthenticationHeaderValue("Bearer", appToken),
                        UserAgent =  { ProductInfoHeaderValue.Parse("SezTestApp") },
                        Accept =     { MediaTypeWithQualityHeaderValue.Parse("application/vnd.github.machine-man-preview+json") }
                    }
                };

                using (HttpResponseMessage response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    JObject obj = JObject.Parse(json);
                    return obj["token"]?.Value<string>();
                }
            }
        }
    }

    static class DateTimeExtensions
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static int ToUnixTimeStamp(this DateTime date)
        {
            return (int)(date - UnixEpoch).TotalSeconds;
        }
    }
}
