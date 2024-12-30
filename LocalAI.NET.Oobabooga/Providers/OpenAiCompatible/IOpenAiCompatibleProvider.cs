using LocalAI.NET.Oobabooga.Models;
using LocalAI.NET.Oobabooga.Models.Chat;

namespace LocalAI.NET.Oobabooga.Providers.OpenAiCompatible
{
    public interface IOpenAiCompatibleProvider : IDisposable
    {
        Task<string> CompleteAsync(string prompt, OobaboogaCompletionOptions? options = null, CancellationToken cancellationToken = default);
        IAsyncEnumerable<string> StreamCompletionAsync(string prompt, OobaboogaCompletionOptions? options = null, CancellationToken cancellationToken = default);
        Task<OobaboogaChatCompletionResponse> ChatCompleteAsync(List<OobaboogaChatMessage> messages, OobaboogaChatCompletionOptions? options = null, CancellationToken cancellationToken = default);
        IAsyncEnumerable<OobaboogaChatMessage> StreamChatCompletionAsync(List<OobaboogaChatMessage> messages, OobaboogaChatCompletionOptions? options = null, CancellationToken cancellationToken = default);
        Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
    }
}