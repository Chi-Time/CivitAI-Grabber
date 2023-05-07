// Copyright (c) 2023 James Johnson
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System.Text.Json;
using System.Text.Json.Serialization;

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
            var value = reader.GetString();

            if (string.IsNullOrWhiteSpace(value))
                return "";
            else
                return value;
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
