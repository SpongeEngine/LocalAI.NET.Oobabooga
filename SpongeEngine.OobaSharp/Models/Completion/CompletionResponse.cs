using Newtonsoft.Json;

namespace SpongeEngine.OobaSharp.Models.Completion
{
    public class CompletionResponse
    {
        [JsonProperty("choices")]
        public List<Choice> Choices { get; set; } = new();

        public class Choice
        {
            [JsonProperty("text")]
            public string Text { get; set; } = string.Empty;
        }
    }
}