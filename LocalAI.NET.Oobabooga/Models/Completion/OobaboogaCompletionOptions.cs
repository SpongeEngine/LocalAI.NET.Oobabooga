namespace LocalAI.NET.Oobabooga.Models
{
    public class OobaboogaCompletionOptions
    {
        public string? ModelName { get; set; }
        public int? MaxTokens { get; set; }
        public float? Temperature { get; set; }
        public float? TopP { get; set; }
        public string[]? StopSequences { get; set; }
    }
}