using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PuiTranslate.Common.Converter
{
    public class StringToIntConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var val = reader.GetString();
            val = val.Trim();

            var ret = convert(val);
            return ret;
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        private int convert(string val)
        {
            int ret = 0;

            if (val.Contains("."))
            {
                val = val.Replace(".", ",");
            }

            if (!int.TryParse(val, out ret))
            {
                if (!double.TryParse(val, out double dblRes))
                {
                    val = val.Replace('.', ',');
                    double.TryParse(val, out dblRes);
                }
                ret = Convert.ToInt32(dblRes);
            }

            return ret;
        }
    }
}
