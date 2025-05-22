namespace BlazorApp1.Client.Utilities.Result.Payloads;

public class ResultPayloadOfType<T>(T payload) 
  : IResultPayload<T> where T : class
{
  [JsonIgnore]
  public bool PayloadExists => Payload != null;

  public T Payload { get; } = payload;

  public static ResultPayloadOfType<T> CreateInstance(T payload)
  {
    return new ResultPayloadOfType<T>(payload);
  }
}

