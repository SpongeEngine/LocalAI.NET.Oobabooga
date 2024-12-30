# LocalAI.NET.Oobabooga
[![NuGet](https://img.shields.io/nuget/v/LocalAI.NET.Oobabooga.svg)](https://www.nuget.org/packages/LocalAI.NET.Oobabooga)
[![NuGet Downloads](https://img.shields.io/nuget/dt/LocalAI.NET.Oobabooga.svg)](https://www.nuget.org/packages/LocalAI.NET.Oobabooga)
[![License](https://img.shields.io/github/license/SpongeEngine/LocalAI.NET.Oobabooga)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-6.0%20%7C%207.0%20%7C%208.0%2B-512BD4)](https://dotnet.microsoft.com/download)

A .NET client library for interacting with Oobabooga's text-generation-webui through its OpenAI-compatible API endpoints. This library provides a simple, efficient way to use local LLMs in your .NET applications.

This package serves as the Oobabooga integration layer for the [LocalAI.NET](https://github.com/SpongeEngine/LocalAI.NET) ecosystem.

## Features
- OpenAI-compatible API support
- Text completion and chat completion
- Streaming responses support
- Character templates and instruction formats
- Comprehensive configuration options
- Built-in error handling and logging
- Cross-platform compatibility
- Full async/await support

📦 [View Package on NuGet](https://www.nuget.org/packages/LocalAI.NET.Oobabooga)

## Installation
Install via NuGet:
```bash
dotnet add package LocalAI.NET.Oobabooga
```

## Quick Start

```csharp
using LocalAI.NET.Oobabooga.Client;
using LocalAI.NET.Oobabooga.Models.Common;
using LocalAI.NET.Oobabooga.Models.Chat;

// Configure the client
var options = new OobaboogaOptions
{
    BaseUrl = "http://localhost:5000",  // Default port for text-generation-webui
    TimeoutSeconds = 120
};

// Create client instance
using var client = new OobaboogaClient(options);

// Simple completion
var response = await client.CompleteAsync(
    "Write a short story about a robot:",
    new OobaboogaCompletionOptions
    {
        MaxTokens = 200,
        Temperature = 0.7f,
        TopP = 0.9f
    });

Console.WriteLine(response);

// Chat completion
var messages = new List<OobaboogaChatMessage>
{
    new() { Role = "user", Content = "Write a poem about coding" }
};

var chatResponse = await client.ChatCompleteAsync(
    messages,
    new OobaboogaChatCompletionOptions
    {
        Mode = "instruct",
        InstructionTemplate = "Alpaca",
        MaxTokens = 200
    });

Console.WriteLine(chatResponse.Choices[0].Message.Content);

// Stream chat completion
await foreach (var message in client.StreamChatCompletionAsync(messages))
{
    Console.Write(message.Content);
}
```

## Configuration Options

### Basic Options
```csharp
var options = new OobaboogaOptions
{
    BaseUrl = "http://localhost:5000",    // text-generation-webui server URL
    ApiKey = "optional_api_key",          // Optional API key for authentication
    TimeoutSeconds = 120                  // Request timeout
};
```

### Chat Completion Options
```csharp
var options = new OobaboogaChatCompletionOptions
{
    ModelName = "optional_model_name",    // Specific model to use
    MaxTokens = 200,                      // Maximum tokens to generate
    Temperature = 0.7f,                   // Randomness (0.0-1.0)
    TopP = 0.9f,                         // Nucleus sampling threshold
    StopSequences = new[] { "\n" },      // Stop sequences
    Mode = "chat",                       // "chat" or "instruct"
    InstructionTemplate = "Alpaca",      // Template for instruction format
    Character = "Assistant"              // Character template to use
};
```

### Text Completion Options
```csharp
var options = new OobaboogaCompletionOptions
{
    ModelName = "optional_model_name",
    MaxTokens = 200,
    Temperature = 0.7f,
    TopP = 0.9f,
    StopSequences = new[] { "\n" }
};
```

## Error Handling
```csharp
try
{
    var response = await client.ChatCompleteAsync(messages, options);
}
catch (OobaboogaException ex)
{
    Console.WriteLine($"Oobabooga error: {ex.Message}");
    Console.WriteLine($"Provider: {ex.Provider}");
    Console.WriteLine($"Status code: {ex.StatusCode}");
    Console.WriteLine($"Response content: {ex.ResponseContent}");
}
catch (Exception ex)
{
    Console.WriteLine($"General error: {ex.Message}");
}
```

## Logging
The client supports Microsoft.Extensions.Logging:

```csharp
ILogger logger = LoggerFactory
    .Create(builder => builder
        .AddConsole()
        .SetMinimumLevel(LogLevel.Debug))
    .CreateLogger<OobaboogaClient>();

var client = new OobaboogaClient(options, logger);
```

## JSON Serialization
Custom JSON settings can be provided:

```csharp
var jsonSettings = new JsonSerializerSettings
{
    NullValueHandling = NullValueHandling.Ignore
};

var client = new OobaboogaClient(options, logger, jsonSettings);
```

## Testing
The library includes both unit and integration tests. Integration tests require a running text-generation-webui server.

To run the tests:
```bash
dotnet test
```

Configure test environment:
```csharp
Environment.SetEnvironmentVariable("OOBABOOGA_BASE_URL", "http://localhost:5000");
```

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing
Contributions are welcome! Please feel free to submit a Pull Request.

## Support
For issues and feature requests, please use the [GitHub issues page](https://github.com/SpongeEngine/LocalAI.NET.Oobabooga/issues).
