using Newtonsoft.Json;

namespace LocalAI.NET.Oobabooga.Models
{
    public class OobaboogaCompletionResponse
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