namespace BlazorApp1.Client.Utilities.Result.Payloads;

public class ResultBoolPayload : IResultPayload<bool>
{
  public bool Payload { get; set; }

  public static IResultPayload<bool> CreateInstance(bool payload)
  {
    return new ResultBoolPayload { Payload = payload };
  }
}



