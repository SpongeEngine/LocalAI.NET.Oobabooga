using System.Net.Http.Headers;
using LocalAI.NET.Oobabooga.Models;
using LocalAI.NET.Oobabooga.Models.Chat;
using LocalAI.NET.Oobabooga.Models.Common;
using LocalAI.NET.Oobabooga.Providers.OpenAiCompatible;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LocalAI.NET.Oobabooga.Client
{
    public class OobaboogaClient : IDisposable
    {
        private readonly IOpenAiCompatibleProvider _openAiCompatibleProvider;
        private readonly OobaboogaOptions _oobaboogaOptions;
        private bool _disposed;

        public string Name => "Oobabooga";
        public string? Version { get; private set; }
        public bool SupportsStreaming => true;

        public OobaboogaClient(OobaboogaOptions oobaboogaOptions, ILogger? logger = null, JsonSerializerSettings? jsonSettings = null)
        {
            _oobaboogaOptions = oobaboogaOptions ?? throw new ArgumentNullException(nameof(oobaboogaOptions));

            JsonSerializerSettings settings = jsonSettings ?? new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri(oobaboogaOptions.BaseUrl),
                Timeout = TimeSpan.FromSeconds(oobaboogaOptions.TimeoutSeconds)
            };

            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("text/event-stream"));

            if (!string.IsNullOrEmpty(oobaboogaOptions.ApiKey))
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {oobaboogaOptions.ApiKey}");
            }

            _openAiCompatibleProvider = new OpenAiCompatibleProvider(httpClient, logger: logger, jsonSettings: settings);
        }

        // Text completion methods
        public Task<string> CompleteAsync(string prompt, OobaboogaCompletionOptions? options = null, CancellationToken cancellationToken = default)
        {
            return _openAiCompatibleProvider.CompleteAsync(prompt, options, cancellationToken);
        }

        public IAsyncEnumerable<string> StreamCompletionAsync(string prompt, OobaboogaCompletionOptions? options = null, CancellationToken cancellationToken = default)
        {
            return _openAiCompatibleProvider.StreamCompletionAsync(prompt, options, cancellationToken);
        }

        // Chat completion methods
        public Task<OobaboogaChatCompletionResponse> ChatCompleteAsync(
            List<OobaboogaChatMessage> messages, 
            OobaboogaChatCompletionOptions? options = null, 
            CancellationToken cancellationToken = default)
        {
            return _openAiCompatibleProvider.ChatCompleteAsync(messages, options, cancellationToken);
        }

        public IAsyncEnumerable<OobaboogaChatMessage> StreamChatCompletionAsync(
            List<OobaboogaChatMessage> messages, 
            OobaboogaChatCompletionOptions? options = null, 
            CancellationToken cancellationToken = default)
        {
            return _openAiCompatibleProvider.StreamChatCompletionAsync(messages, options, cancellationToken);
        }

        // Health check method
        public Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
        {
            return _openAiCompatibleProvider.IsAvailableAsync(cancellationToken);
        }

        // Helper method to create a chat message
        public static OobaboogaChatMessage CreateChatMessage(string role, string content)
        {
            return new OobaboogaChatMessage
            {
                Role = role,
                Content = content
            };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _openAiCompatibleProvider?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}