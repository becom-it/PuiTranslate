using PuiTranslate.Common.Converter;
using System.Text.Json.Serialization;

namespace PuiTranslate.Common.Models
{
    public class TranslationEntry
    {
        [JsonPropertyName("TRID"), JsonConverter(typeof(StringToIntConverter))]
        public int Id { get; set; }

        [JsonPropertyName("DELANG")]
        public string DeLangCode { get; set; }

        [JsonPropertyName("DETEXT")]
        public string DeText { get; set; }

        [JsonPropertyName("ENLANG")]
        public string EnLangCode { get; set; }

        [JsonPropertyName("ENTEXT")]
        public string EnText { get; set; }

        public int GroupId { get; set; }
    }
}
