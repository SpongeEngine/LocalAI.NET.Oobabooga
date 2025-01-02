using SpongeEngine.OobaSharp.Models.Chat;
using SpongeEngine.OobaSharp.Models.Completion;

namespace SpongeEngine.OobaSharp.Providers.OpenAiCompatible
{
    public interface IOpenAiCompatibleProvider : IDisposable
    {
        Task<string> CompleteAsync(string prompt, CompletionOptions? options = null, CancellationToken cancellationToken = default);
        IAsyncEnumerable<string> StreamCompletionAsync(string prompt, CompletionOptions? options = null, CancellationToken cancellationToken = default);
        Task<ChatCompletionResponse> ChatCompleteAsync(List<ChatMessage> messages, ChatCompletionOptions? options = null, CancellationToken cancellationToken = default);
        IAsyncEnumerable<ChatMessage> StreamChatCompletionAsync(List<ChatMessage> messages, ChatCompletionOptions? options = null, CancellationToken cancellationToken = default);
        Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
    }
}