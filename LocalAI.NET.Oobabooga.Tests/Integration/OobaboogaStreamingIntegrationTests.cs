using System.Net.Http.Headers;
using System.Text;
using FluentAssertions;
using LocalAI.NET.Oobabooga.Client;
using LocalAI.NET.Oobabooga.Models;
using LocalAI.NET.Oobabooga.Models.Chat;
using LocalAI.NET.Oobabooga.Models.Common;
using LocalAI.NET.Oobabooga.Tests.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace LocalAI.NET.Oobabooga.Tests.Integration
{
    public class OobaboogaStreamingIntegrationTests : IntegrationTestBase
    {
        
        private readonly OobaboogaClient _client;
        private readonly ILogger<OobaboogaStreamingIntegrationTests> _logger;


        public OobaboogaStreamingIntegrationTests(ITestOutputHelper output) : base(output)
        {
            _logger = LoggerFactory
                .Create(builder => builder.AddXUnit(output))
                .CreateLogger<OobaboogaStreamingIntegrationTests>();

            _client = new OobaboogaClient(new OobaboogaOptions
            {
                BaseUrl = TestConfig.BaseApiUrl
            }, _logger);
        }

        [SkippableFact]
        [Trait("Category", "Integration")]
        public async Task StreamChatCompletion_ShouldWork()
        {
            Skip.If(!ServerAvailable, "API endpoint not available");

            // Arrange
            var messages = new List<OobaboogaChatMessage>
            {
                new() { Role = "user", Content = "Tell me a very short story about a cat." }
            };
            var options = new OobaboogaChatCompletionOptions
            {
                Mode = "instruct",
                InstructionTemplate = "Alpaca",
                MaxTokens = 100,
                Temperature = 0.7f,
                TopP = 0.9f
            };

            // Act
            var chatMessages = new List<OobaboogaChatMessage>();
            await foreach (var message in _client.StreamChatCompletionAsync(messages, options))
            {
                _logger.LogInformation("Received message: {Content}", message.Content);
                chatMessages.Add(message);
            }

            // Assert
            chatMessages.Should().NotBeEmpty("we should receive at least one message");
            chatMessages.Should().OnlyContain(m => !string.IsNullOrWhiteSpace(m.Content), 
                "all messages should have content");
        }

        [SkippableFact]
        [Trait("Category", "Integration")]
        public async Task StreamCompletion_WithTimeout_ShouldComplete()
        {
            Skip.If(!ServerAvailable, "API endpoint not available");

            // Arrange
            var options = new OobaboogaCompletionOptions
            {
                MaxTokens = 20,
                Temperature = 0.7f,
                TopP = 0.9f
            };

            // Act
            var tokens = new List<string>();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            try
            {
                await foreach (var token in _client.StreamCompletionAsync(
                    "Write a short story about",
                    options,
                    cts.Token))
                {
                    tokens.Add(token);
                    Output.WriteLine($"Received token: {token}");

                    if (tokens.Count >= options.MaxTokens)
                        break;
                }
            }
            catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
            {
                Output.WriteLine($"Stream timed out after receiving {tokens.Count} tokens");
            }

            // Assert
            tokens.Should().NotBeEmpty();
            string.Concat(tokens).Should().NotBeNullOrEmpty();
        }
    }
}