using FluentAssertions;
using LocalAI.NET.Oobabooga.Models.Chat;
using LocalAI.NET.Oobabooga.Providers.OpenAiCompatible;
using LocalAI.NET.Oobabooga.Tests.Common;
using Newtonsoft.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;
using Xunit.Abstractions;

namespace LocalAI.NET.Oobabooga.Tests.Unit.Providers.OpenAiCompatible
{
    public class OpenAiCompatibleProviderTests : UnitTestBase
    {
        private readonly OpenAiCompatibleProvider _provider;
        private readonly HttpClient _httpClient;

        public OpenAiCompatibleProviderTests(ITestOutputHelper output) : base(output)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _provider = new OpenAiCompatibleProvider(_httpClient, logger: Logger);
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
            var expectedResponse = new OobaboogaChatCompletionResponse
            {
                Choices = new List<ChatCompletionChoice>
                {
                    new()
                    {
                        Message = new OobaboogaChatMessage 
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
                new List<OobaboogaChatMessage> { new() { Role = "user", Content = "Test" } },
                new OobaboogaChatCompletionOptions { Mode = "instruct" });

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