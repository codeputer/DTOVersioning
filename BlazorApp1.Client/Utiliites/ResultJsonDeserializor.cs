
namespace BlazorApp1.Client.Utilities;

public class ResultJsonDeserializor : JsonConverter<Result<object>>
{
    public override Result<object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;

            bool resultStatus = root.GetProperty("ResultStatus").GetBoolean();
            string fullTypeName = root.GetProperty("FullTypeName").GetString() ?? "Unknown";
            Type resultType = Type.GetType(fullTypeName) ?? typeof(object);

            object? resultValue = null;
            if (root.TryGetProperty("ResultValue", out JsonElement resultValueElement))
            {
                resultValue = JsonSerializer.Deserialize(resultValueElement.GetRawText(), resultType);
            }

            List<string> messages = JsonSerializer.Deserialize<List<string>>(root.GetProperty("Messages").GetRawText(), options) ?? [];
            List<Exception> exceptions = JsonSerializer.Deserialize<List<Exception>>(root.GetProperty("Exceptions").GetRawText(), options) ?? [];

            if (resultStatus)
            {
                return Result<object>.Success(resultValue!, message: messages.FlattenList(), fullTypeName: fullTypeName);
            }

            return Result<object>.Failure(messages, exceptions);
        }
    }

    public override void Write(Utf8JsonWriter writer, Result<object> value, JsonSerializerOptions options)
    {
        throw new NotImplementedException("Serialization is not implemented.");
    }
}

