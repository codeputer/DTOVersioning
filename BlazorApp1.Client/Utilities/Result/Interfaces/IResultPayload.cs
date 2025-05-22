namespace BlazorApp1.Client.Utilities.Result.Interfaces;

public interface IResultPayload<out T>
{
  [JsonIgnore]
  bool PayloadExists => Payload != null;

  /// <summary>
  /// Must store exactly one payload of type T.
  /// </summary>
  T Payload { get; }
}
