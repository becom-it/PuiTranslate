using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PuiTranslate.Common.Models
{
    public class Response<T>
    {
        [JsonPropertyName("metrics")]
        public Metrics Metrics { get; set; }
        [JsonPropertyName("data")]
        public List<T> Data { get; set; }
    }

    public class Metrics
    {
        [JsonPropertyName("took")]
        public string Took { get; set; }
        [JsonPropertyName("sqlTime")]
        public string SqlTime { get; set; }
    }
}
