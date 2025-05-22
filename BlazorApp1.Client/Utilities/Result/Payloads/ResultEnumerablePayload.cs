namespace BlazorApp1.Client.Utilities.Result.Payloads;

public class ResultEnumerablePayload<T> : IResultPayload<IEnumerable<T>> where T: class, new()
{
    public IEnumerable<T>? Payload { get; set; }

    public static IResultPayload<IEnumerable<T>> CreateInstance(IEnumerable<T> payload)
    {
        return new ResultEnumerablePayload<T> { Payload = payload };
    }
}
