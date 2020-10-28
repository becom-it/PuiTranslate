using PuiTranslate.Common.Converter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PuiTranslate.Common.Models
{
    public class IntResponse
    {
        [JsonPropertyName("NEWID"), JsonConverter(typeof(StringToIntConverter))]
        public int NewId { get; set; }
    }
}
