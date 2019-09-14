using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ChristmasPi.Data.Models {
    public enum HardwareType {
        RPI_WS281x,
        TEST_RENDER,
        UNKNOWN
    }
    // from https://stackoverflow.com/a/52526628
    public class HardwareTypeConverter : JsonConverter<HardwareType> {
        public override HardwareType ReadJson(JsonReader reader, Type objectType, HardwareType existingValue, bool hasExistingValue, JsonSerializer serializer) {
            var token = reader.Value as string ?? reader.Value.ToString();
            var stripped = Regex.Replace(token, @"<[^>]+>", string.Empty);
            if (Enum.TryParse<HardwareType>(stripped, out var result)) {
                return result;
            }
            return default(HardwareType);
        }

        public override void WriteJson(JsonWriter writer, HardwareType value, JsonSerializer serializer) {
            writer.WriteValue(value.ToString());
        }
    }
}