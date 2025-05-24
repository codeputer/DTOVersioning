namespace BlazorApp1.Client.Utilities.Result.Payloads;

public class ResultEnumerablePayload<T>
  : IResultPayload<IEnumerable<T>> where T : class
{
  public IEnumerable<T> Payload { get; private set; } = default!;

  public static ResultEnumerablePayload<T> CreateInstance(IEnumerable<T> payload)
  {
    return new ResultEnumerablePayload<T> { Payload =  payload };
  }
}


