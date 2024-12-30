using System.Net.Http.Headers;
using LocalAI.NET.Oobabooga.Models.Common;

namespace LocalAI.NET.Oobabooga.Utils
{
    public static class HttpClientFactory
    {
        public static HttpClient Create(OobaboogaOptions oobaboogaOptions)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(oobaboogaOptions.BaseUrl),
                Timeout = TimeSpan.FromSeconds(oobaboogaOptions.TimeoutSeconds)
            };

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("text/event-stream"));

            if (!string.IsNullOrEmpty(oobaboogaOptions.ApiKey))
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {oobaboogaOptions.ApiKey}");
            }

            return client;
        }
    }
}