namespace BlazorApp1.Client.Utilities.Result.Payloads;

public class ResultPayloadOfType<T>
  : IResultPayload<T> where T : class
{
  public T Payload { get; private set; } = default!;

  public static ResultPayloadOfType<T> CreateInstance(T payload)
  {
    return new ResultPayloadOfType<T> { Payload = payload };
  }
}

