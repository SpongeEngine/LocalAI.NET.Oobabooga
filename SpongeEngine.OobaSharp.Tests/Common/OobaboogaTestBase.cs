using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SpongeEngine.OobaSharp.Providers.OpenAiCompatible;
using Xunit;
using Xunit.Abstractions;

namespace SpongeEngine.OobaSharp.Tests.Common
{
    public abstract class OobaboogaTestBase : IAsyncLifetime
    {
        protected readonly IOpenAiCompatibleProvider Provider;
        protected readonly ITestOutputHelper Output;
        protected readonly ILogger Logger;
        protected bool ServerAvailable;

        protected OobaboogaTestBase(ITestOutputHelper output)
        {
            Output = output;
            Logger = LoggerFactory
                .Create(builder => builder.AddXUnit(output))
                .CreateLogger(GetType());

            var httpClient = new HttpClient 
            { 
                BaseAddress = new Uri(TestConfig.BaseApiUrl),
                Timeout = TimeSpan.FromSeconds(TestConfig.TimeoutSeconds)
            };

            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            Provider = new OobaSharpOpenAiCompatibleProvider(httpClient, logger: Logger, jsonSettings: jsonSettings);
        }

        public async Task InitializeAsync()
        {
            try
            {
                ServerAvailable = await Provider.IsAvailableAsync();
                if (ServerAvailable)
                {
                    Output.WriteLine("OpenAI API endpoint is available");
                }
                else
                {
                    Output.WriteLine("OpenAI API endpoint is not available");
                    throw new SkipTestException("OpenAI API endpoint is not available");
                }
            }
            catch (Exception ex) when (ex is not SkipTestException)
            {
                Output.WriteLine($"Failed to connect to OpenAI API endpoint: {ex.Message}");
                throw new SkipTestException("Failed to connect to OpenAI API endpoint");
            }
        }

        public Task DisposeAsync()
        {
            if (Provider is IDisposable disposable)
            {
                disposable.Dispose();
            }
            return Task.CompletedTask;
        }

        private class SkipTestException : Exception
        {
            public SkipTestException(string message) : base(message) { }
        }
    }
}