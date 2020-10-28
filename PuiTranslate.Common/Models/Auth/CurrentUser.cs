using System.Text.Json.Serialization;

namespace PuiTranslate.Common.Models.Auth
{
    public class CurrentUser
    {
        [JsonPropertyName("AUTHORIZATION_NAME")]
        public string UserName { get; set; }
        [JsonPropertyName("STATUS")]
        public string UserStatus { get; set; }
        [JsonPropertyName("TEXT_DESCRIPTION")]
        public string Description { get; set; }
        [JsonPropertyName("authState")]
        public bool IsAuthenticated { get; set; }
        [JsonPropertyName("authString")]
        public string AuthString { get; set; }
    }
}
