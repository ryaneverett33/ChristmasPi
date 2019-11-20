using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ChristmasPi.Data.Models {
    [JsonConverter(typeof(HardwareTypeConverter))]
    public enum HardwareType {
        RPI_WS281x,
        TEST_RENDER,
        UNKNOWN
    }

    /// <summary>
    /// A JsonConverter to parse between HardwareType and JSON
    /// </summary>
    /// <remarks>Only used by JSON.Net, no need to call directly</remarks>
    /// <see cref="https://stackoverflow.com/a/52526628">
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