namespace LocalAI.NET.Oobabooga.Models.Common
{
    public class OobaboogaException : Exception
    {
        public string Provider { get; }
        public int StatusCode { get; }
        public string ResponseContent { get; }

        public OobaboogaException(string message, string provider, int statusCode, string responseContent) 
            : base(message)
        {
            Provider = provider;
            StatusCode = statusCode;
            ResponseContent = responseContent;
        }
    }
}