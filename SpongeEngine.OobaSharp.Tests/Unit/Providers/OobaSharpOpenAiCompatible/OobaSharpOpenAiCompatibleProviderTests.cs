using FluentAssertions;
using Newtonsoft.Json;
using SpongeEngine.OobaSharp.Models.Chat;
using SpongeEngine.OobaSharp.Providers.OpenAiCompatible;
using SpongeEngine.OobaSharp.Tests.Common;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;
using Xunit.Abstractions;

namespace SpongeEngine.OobaSharp.Tests.Unit.Providers.OobaSharpOpenAiCompatible
{
    public class OobaSharpOpenAiCompatibleProviderTests : UnitTestBase
    {
        private readonly OobaSharpOpenAiCompatibleProvider _provider;
        private readonly HttpClient _httpClient;

        public OobaSharpOpenAiCompatibleProviderTests(ITestOutputHelper output) : base(output)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _provider = new OobaSharpOpenAiCompatibleProvider(_httpClient, logger: Logger);
        }

        [Fact]
        public async Task CompleteAsync_ShouldReturnValidResponse()
        {
            // Arrange
            const string expectedResponse = "Test response";
            Server
                .Given(Request.Create()
                    .WithPath("/v1/completions")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody($"{{\"choices\": [{{\"text\": \"{expectedResponse}\"}}]}}"));

            // Act
            var response = await _provider.CompleteAsync("Test prompt");

            // Assert
            response.Should().Be(expectedResponse);
        }

        [Fact]
        public async Task ChatCompleteAsync_ShouldHandleInstructMode()
        {
            // Arrange
            var expectedResponse = new ChatCompletionResponse
            {
                Choices = new List<ChatCompletionChoice>
                {
                    new()
                    {
                        Message = new ChatMessage 
                        { 
                            Role = "assistant", 
                            Content = "Response" 
                        }
                    }
                }
            };

            Server
                .Given(Request.Create()
                    .WithPath("/v1/chat/completions")
                    .WithBody(body => body.Contains("\"mode\":\"instruct\""))
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody(JsonConvert.SerializeObject(expectedResponse)));

            // Act
            var response = await _provider.ChatCompleteAsync(
                new List<ChatMessage> { new() { Role = "user", Content = "Test" } },
                new ChatCompletionOptions { Mode = "instruct" });

            // Assert
            response.Choices[0].Message.Content.Should().Be("Response");
        }

        [Fact]
        public async Task IsAvailableAsync_WhenServerResponds_ShouldReturnTrue()
        {
            // Arrange
            Server
                .Given(Request.Create()
                    .WithPath("/v1/models")
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200));

            // Act
            var isAvailable = await _provider.IsAvailableAsync();

            // Assert
            isAvailable.Should().BeTrue();
        }

        public override void Dispose()
        {
            _httpClient.Dispose();
            base.Dispose();
        }
    }
}