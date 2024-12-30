using FluentAssertions;
using LocalAI.NET.Oobabooga.Client;
using LocalAI.NET.Oobabooga.Models;
using LocalAI.NET.Oobabooga.Models.Chat;
using LocalAI.NET.Oobabooga.Models.Common;
using LocalAI.NET.Oobabooga.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace LocalAI.NET.Oobabooga.Tests.Integration
{
    public class OobaboogaIntegrationTests : IntegrationTestBase
    {
        private readonly OobaboogaClient _client;

        public OobaboogaIntegrationTests(ITestOutputHelper output) : base(output)
        {
            _client = new OobaboogaClient(new OobaboogaOptions
            {
                BaseUrl = TestConfig.BaseApiUrl
            }, Logger);
        }

        [SkippableFact]
        [Trait("Category", "Integration")]
        public async Task Complete_WithSimplePrompt_ShouldWork()
        {
            Skip.If(!ServerAvailable, "API endpoint not available");

            // Arrange & Act
            var response = await _client.CompleteAsync(
                "Once upon a time",
                new OobaboogaCompletionOptions
                {
                    MaxTokens = 20,
                    Temperature = 0.7f,
                    TopP = 0.9f
                });

            // Assert
            response.Should().NotBeNullOrEmpty();
            Output.WriteLine($"Completion response: {response}");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task ChatComplete_WithInstructTemplate_ShouldWork()
        {
            Skip.If(!ServerAvailable, "API endpoint not available");

            // Arrange
            var messages = new List<OobaboogaChatMessage>
            {
                new() { Role = "user", Content = "Write a short story" }
            };

            // Act
            var response = await _client.ChatCompleteAsync(
                messages,
                new OobaboogaChatCompletionOptions
                {
                    Mode = "instruct",
                    InstructionTemplate = "Alpaca",
                    MaxTokens = 100
                });

            // Assert
            response.Should().NotBeNull();
            response.Choices.Should().NotBeNull();
            response.Choices.Should().NotBeEmpty("API should return at least one choice");
            response.Choices.First().Message.Should().NotBeNull("Choice should contain a message");
            response.Choices.First().Message.Content.Should().NotBeNullOrEmpty("Message should contain content");
        }
    }
}