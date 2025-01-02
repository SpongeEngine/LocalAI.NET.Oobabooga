using System.Net.Http.Headers;
using SpongeEngine.OobaboogaSharp.Models.Common;

namespace SpongeEngine.OobaboogaSharp.Utils
{
    public static class HttpClientFactory
    {
        public static HttpClient Create(OobaSharpOptions oobaSharpOptions)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(oobaSharpOptions.BaseUrl),
                Timeout = TimeSpan.FromSeconds(oobaSharpOptions.TimeoutSeconds)
            };

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("text/event-stream"));

            if (!string.IsNullOrEmpty(oobaSharpOptions.ApiKey))
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {oobaSharpOptions.ApiKey}");
            }

            return client;
        }
    }
}