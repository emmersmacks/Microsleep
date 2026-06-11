using System;
using CutTwice.Gameplay.Runtime.Chunks.Serialization.SimpleTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CutTwice.Gameplay.Runtime.Chunks.Serialization
{
    public class SimplePositionJsonConverter : JsonConverter<SimplePosition>
    {
        public override SimplePosition ReadJson(
            JsonReader reader,
            Type objectType,
            SimplePosition existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                string presetKey = (string)reader.Value;

                if (PositionPresets.TryGet(presetKey, out var preset))
                {
                    return preset;
                }

                throw new JsonSerializationException(
                    $"Unknown position preset: {presetKey}");
            }

            JObject obj = JObject.Load(reader);

            return obj.ToObject<SimplePosition>();
        }

        public override void WriteJson(
            JsonWriter writer,
            SimplePosition value,
            JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}