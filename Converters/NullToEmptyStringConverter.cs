using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CivitAI_Grabber.Converters
{
    public class NullToEmptyStringConverter : JsonConverter<string>
    {
        public override bool HandleNull => true;

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(string);
        }

        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var res = reader.GetString();

            if (string.IsNullOrWhiteSpace(res))
                return "";
            else
                return res;
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (value == null)
                writer.WriteStringValue("");
            else
                writer.WriteStringValue(value);
        }
    }
}
