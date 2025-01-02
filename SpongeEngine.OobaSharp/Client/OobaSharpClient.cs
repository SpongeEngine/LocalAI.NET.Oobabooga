using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SpongeEngine.OobaSharp.Models.Chat;
using SpongeEngine.OobaSharp.Models.Common;
using SpongeEngine.OobaSharp.Models.Completion;
using SpongeEngine.OobaSharp.Providers.OpenAiCompatible;

namespace SpongeEngine.OobaSharp.Client
{
    public class OobaSharpClient : IDisposable
    {
        private readonly IOpenAiCompatibleProvider _openAiCompatibleProvider;
        private readonly OobaSharpOptions _oobaSharpOptions;
        private bool _disposed;

        public string Name { get; set; }
        public string? Version { get; private set; }
        public bool SupportsStreaming => true;

        public OobaSharpClient(OobaSharpOptions oobaSharpOptions, ILogger? logger = null, JsonSerializerSettings? jsonSettings = null)
        {
            _oobaSharpOptions = oobaSharpOptions ?? throw new ArgumentNullException(nameof(oobaSharpOptions));

            JsonSerializerSettings settings = jsonSettings ?? new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri(oobaSharpOptions.BaseUrl),
                Timeout = TimeSpan.FromSeconds(oobaSharpOptions.TimeoutSeconds)
            };

            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("text/event-stream"));

            if (!string.IsNullOrEmpty(oobaSharpOptions.ApiKey))
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {oobaSharpOptions.ApiKey}");
            }

            _openAiCompatibleProvider = new OobaSharpOpenAiCompatibleProvider(httpClient, logger: logger, jsonSettings: settings);
        }

        // Text completion methods
        public Task<string> CompleteAsync(string prompt, CompletionOptions? options = null, CancellationToken cancellationToken = default)
        {
            return _openAiCompatibleProvider.CompleteAsync(prompt, options, cancellationToken);
        }

        public IAsyncEnumerable<string> StreamCompletionAsync(string prompt, CompletionOptions? options = null, CancellationToken cancellationToken = default)
        {
            return _openAiCompatibleProvider.StreamCompletionAsync(prompt, options, cancellationToken);
        }

        // Chat completion methods
        public Task<ChatCompletionResponse> ChatCompleteAsync(
            List<ChatMessage> messages, 
            ChatCompletionOptions? options = null, 
            CancellationToken cancellationToken = default)
        {
            return _openAiCompatibleProvider.ChatCompleteAsync(messages, options, cancellationToken);
        }

        public IAsyncEnumerable<ChatMessage> StreamChatCompletionAsync(
            List<ChatMessage> messages, 
            ChatCompletionOptions? options = null, 
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
        public static ChatMessage CreateChatMessage(string role, string content)
        {
            return new ChatMessage
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