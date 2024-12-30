using FluentAssertions;
using LocalAI.NET.Oobabooga.Client;
using LocalAI.NET.Oobabooga.Models;
using LocalAI.NET.Oobabooga.Models.Chat;
using LocalAI.NET.Oobabooga.Models.Common;
using LocalAI.NET.Oobabooga.Tests.Common;
using Newtonsoft.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;
using Xunit.Abstractions;

namespace LocalAI.NET.Oobabooga.Tests.Unit.Client
{
    public class OobaboogaClientTests : UnitTestBase
    {
        private readonly OobaboogaClient _client;

        public OobaboogaClientTests(ITestOutputHelper output) : base(output)
        {
            _client = new OobaboogaClient(new OobaboogaOptions
            {
                BaseUrl = BaseUrl
            }, Logger);
        }

        [Fact]
        public async Task Complete_WithBasicPrompt_ShouldWork()
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
            var response = await _client.CompleteAsync("Test prompt");

            // Assert
            response.Should().Be(expectedResponse);
        }

        [Fact]
        public async Task ChatComplete_WithCharacter_ShouldWork()
        {
            // Arrange
            var expectedResponse = new OobaboogaChatCompletionResponse
            {
                Id = "test",
                Choices = new List<ChatCompletionChoice>
                {
                    new()
                    {
                        Message = new OobaboogaChatMessage { Role = "assistant", Content = "Hello!" }
                    }
                }
            };

            Server
                .Given(Request.Create()
                    .WithPath("/v1/chat/completions")
                    .WithBody(body => body.Contains("\"character\":\"Example\""))
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody(JsonConvert.SerializeObject(expectedResponse)));

            // Act
            var response = await _client.ChatCompleteAsync(
                new List<OobaboogaChatMessage> { new() { Role = "user", Content = "Hi" } },
                new OobaboogaChatCompletionOptions { Character = "Example" });

            // Assert
            response.Choices[0].Message.Content.Should().Be("Hello!");
        }

        [Fact]
        public async Task Complete_WithError_ShouldThrowException()
        {
            // Arrange
            Server
                .Given(Request.Create()
                    .WithPath("/v1/completions")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(500)
                    .WithBody("Internal server error"));

            // Act & Assert
            var act = () => _client.CompleteAsync("Test prompt");
            await act.Should().ThrowAsync<OobaboogaException>()
                .WithMessage("Completion request failed");
        }

        [Fact]
        public async Task Complete_WithCancellation_ShouldCancel()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            Server
                .Given(Request.Create()
                    .WithPath("/v1/completions")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithBody("...")
                    .WithDelay(TimeSpan.FromSeconds(5)));

            // Act & Assert
            var completeTask = _client.CompleteAsync("Test prompt", cancellationToken: cts.Token);
            cts.Cancel();
    
            await Assert.ThrowsAsync<TaskCanceledException>(() => completeTask);
        }
    }
}