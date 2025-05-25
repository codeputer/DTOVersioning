namespace BlazorApp1.Client.Utilities.Result;

/// <summary>
/// The Result class is a generic utility designed to standardize the way method call outcomes are represented and handled throughout the application.
/// 
/// It encapsulates:
/// - Operation Status: Indicates if an operation was successful or failed (ResultStatus, IsSuccessful, Failed).
/// - Payload: Holds the result value or data from the operation (PayloadWrapper).
/// - Error Handling: Collects error messages and exceptions that occurred during the operation (Messages, Exceptions).
/// - Serialization Support: Provides mechanisms to serialize and deserialize the result and its payload for transport or storage (ResultJson).
/// - Type Information: Stores the full type name of the result for polymorphic scenarios (FullTypeName).
/// - Factory Methods: Supplies static methods to easily create success or failure results, supporting consistent error and result handling patterns.
/// 
/// <para>
/// The class uses two generic type parameters:
/// <list type="bullet">
/// <item><b>TReturn</b>: The logical or expected type of the result (e.g., a DTO or domain object) that consumers expect to work with.</item>
/// <item><b>TResultPayload</b>: The actual payload wrapper type, which must implement IResultPayload&lt;TReturn&gt;. This allows for additional metadata, structure, or polymorphic handling around the core result value.</item>
/// </list>
/// This dual-generic approach allows the Result class to be flexible and type-safe, supporting scenarios where the payload may need to wrap, extend, or add metadata to the core result type. 
/// It enables advanced patterns such as versioning, polymorphic deserialization, or result decoration, while still enforcing that the payload is compatible with the expected result type.
/// </para>
/// <para>
/// <b>Example:</b> TReturn could be ICustomerDTO, and TResultPayload could be ResultPayloadOfType&lt;CustomerV1&gt;, allowing the result to carry both the expected interface and a strongly-typed payload with extra context.
/// </para>
/// 
/// This class helps enforce a consistent, robust, and testable approach to error handling, result reporting, and data transport across the Blazor application.
/// </summary>
public class Result<TReturn, TResultPayload>
  where TResultPayload : IResultPayload<TReturn>
{
  // This class is used to return a result from a method call, and it contains the following properties:
  // - ResultStatus: a boolean value that indicates if the operation was successful or not
  // - ResultValue: reflects the value of type T when method call returned a ResultStatus==true or Successful()
  // - Messages: a list of messages that can be used to provide additional information about the operation
  // - Exceptions: a list of exceptions that can be used to provide additional information about the operation


  [JsonPropertyName(nameof(FullTypeName))]
  public string FullTypeName { get; private set; } = typeof(TReturn).FullName ?? "Unknown";

  /// <summary>
  /// ResultStatus is a boolean value that indicates if the operation was successful or not
  /// A successful operation is a ResultStatus==true, and the ResultValue is not null
  /// </summary>
  [JsonPropertyName(nameof(ResultStatus))]
  public bool ResultStatus { get; private set; }

  /// <summary>
  /// Success is a ResultStatus==true, and the ResultValue is not null
  /// </summary>
  [JsonIgnore]
  public bool IsSuccessful => ResultStatus == true && PayloadWrapper is not null && PayloadWrapper is not null;

  /// <summary>
  /// Failed is a ResultStatus==false, and the ResultValue may be null - check Messages or Exceptions for more information
  /// </summary>
  [JsonIgnore]
  public bool Failed => ResultStatus == false;

  /// <summary>
  /// Reflects the value of type T when method call returned a ResultStatus==true or Successful()
  /// </summary>
  [JsonIgnore]
  public TResultPayload? PayloadWrapper { get; private set; }

  [JsonPropertyName(nameof(ResultJson))]
  public string? ResultJson { get; private set; }


  public string? Serialize()
  {
    if (this.IsSuccessful)
    {
      this.ResultJson = JsonSerializer.Serialize(this.PayloadWrapper!.Payload);
    }
    else
    {
      this.ResultJson = JsonSerializer.Serialize(this.Messages);
    }

    return this.ResultJson;
  }

  public void Deserialize()
  {

    if (string.IsNullOrWhiteSpace(this.ResultJson))
    {
      Messages.Add("ResultJson is null or empty, cannot deserialize.");
      this.ResultJson = string.Join("|", Messages);
      return;
    }

    try
    {
      PayloadWrapper = JsonSerializer.Deserialize<TResultPayload>(this.ResultJson);
    }
    catch (JsonException ex)
    {
      Exceptions.Add(ex);
      Messages.Add($"Error de-serializing ResultJson: {ex.Message}");
    }
  }


  [JsonPropertyName(nameof(Messages))]
  public List<string> Messages { get; set; } = []; //must have setter for deserialization

  [JsonPropertyName(nameof(Exceptions))]
  public List<Exception> Exceptions { get; set; } = []; //must have setter for deserialization

  [JsonConstructor]
  public Result(
    bool resultStatus,
    string resultJson,
    List<string>? messages = null,
    List<Exception>? exceptions = null,
    string? fullTypeName = null)
  {
    ResultStatus = resultStatus;
    ResultJson = resultJson;
    Messages = messages ?? [];
    Exceptions = exceptions ?? [];
    FullTypeName = fullTypeName ?? typeof(TReturn).FullName ?? "Unknown";
  }

  // Internal constructor for internal use only (no longer needs internalConstructor parameter)
  private Result(
    bool isSuccess,
    TResultPayload? value,
    string[]? messages = null,
    Exception[]? exceptions = null,
    string? fullTypeName = null)
  {
    if (typeof(TReturn) is not null && fullTypeName is not null)
    {
      FullTypeName = fullTypeName;
    }
    else
    {
      FullTypeName = typeof(TReturn).FullName ?? "Unknown";
    }

    ResultStatus = isSuccess;
    PayloadWrapper = value;

    // If messages are provided, use them. Otherwise, use the message within the Exception.
    switch (messages)
    {
      case not null:
        Messages = [.. messages];
        break;
      default:
        if (exceptions is not null)
        {
          Messages = [.. exceptions.Select(e => $"Exception Msg:{e.Message}")];
        }
        else
        {
          Messages = [];
        }
        break;
    }

    if (Failed && Messages.Count == 0)
    {
      Messages.Add("Error: Result was a failure but no error messages were provided. A failure should always have error messages");
    }

    if (ResultStatus && PayloadWrapper is null)
    {
#if DEBUG
      // In debug mode, throw an exception if ResultStatus is true but ResultValue is null
      System.Diagnostics.Debug.Assert(IsSuccessful == false, "ResultStatus is true but PayloadWrapper is null. This should not happen.");
#endif
      Messages.Add("Warning!! Result is True, but the PayloadWrapper is null - this is an Assertion on the Result class as Success should always return a not null Result.");
    }
    //note: ensure that we don't carry the exceptions in production code to 

#if DEBUG
    if (exceptions is not null)
    {
      Exceptions.AddRange(exceptions);
    }
#else
    if (exceptions is not null && exceptions.Length > 0)
    {
      Exceptions.Clear();
      //just to ensure we leave a bread crub
      Exceptions.Add(new Exception("Exceptions have occurred - please report his message to technical support"));
    }
#endif
  }


  #region StaticFactoryMethods
  ///// <summary>
  ///// Checks the number of changes that have occurred, if greater than zero, then ResultStatus is true, otherwise false
  ///// </summary>
  ///// <param name="numberOfChanges"></param>
  ///// <param name="failureMessage"></param>
  ///// <returns></returns>
  //public static Result<TReturn> SaveChangesActivity(
  //  int numberOfChanges,
  //  string failureMessage = "change tracking detected no changes"
  //)
  //{
  //  //return Result<ResultBoolPayload>.CheckCondition(ResultBoolPayload.StorePayload(numberOfChanges > 0), failureMessage);
  //  return Result<TReturn>.CheckCondition(validationTest: () => numberOfChanges > 0, failureMessage);
  //}

  ///// <summary>
  ///// Using the condition, will determine if Result.Success is provided, or Result.Failure is provided
  ///// </summary>
  ///// <param name="validationTest"></param>
  ///// <param name="messages"></param>
  ///// <returns></returns>
  //public static Result<TReturn> CheckCondition(
  //  Expression<Func<bool>> validationTest,
  //  params string[] messages
  //)
  //{
  //  Func<bool> compiledExpression = validationTest.Compile();
  //  IResultPayload<bool> resultBoolPayload = ResultBoolPayload.CreateInstance(compiledExpression());
  //  if (resultBoolPayload.Payload)
  //    return Result<TReturn>.Success(compiledExpression(), messages);
  //  else
  //    return Result<TReturn>.Failure(messages: messages);
  //}


  /// <summary>
  /// The condition is checked, and if the condition is true, then the value is returned as a success
  /// 
  /// var Result = ResultHelper.CheckCondition(name, () => name.Length > 5, "Failure message");
  /// </summary>
  /// <param name="value">if condition is successful, value should not be null</param>
  /// <param name="validationTest"></param>
  /// <param name="messages"></param>
  /// <returns></returns>
  //public static Result<TReturn> CheckCondition(
  //  TResultPayload? value,
  //  Expression<Func<bool>> validationTest,
  //  params string[] messages
  //)
  //{
  //  if (validationTest.Compile()()) // Compile() first then Execute the function () - reason for ()()
  //  {
  //    if (value == null)
  //      throw new ArgumentNullException(nameof(value), "Value should not be null if the condition is successful");

  //    return Result<TReturn>.Success(value: value!);
  //  }

  //  // Extract text representation of the expression
  //  string conditionText = validationTest.Body.ToString();
  //  string defaultMessage = $"Condition failed: {conditionText}";

  //  return Result<TReturn>.Failure(messages: messages.Length > 0 ? messages : [defaultMessage]);
  //}

  /// <summary>
  /// Success is an indication that the return value was a successful operation, and the value is provided in the generic type provided
  /// However, if the type is bool, and the Resulting value is false - is it a successful operation?
  /// The override must be use if the ResultValue is true, and the ResultValue is false.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="message"></param>
  /// <param name="isSuccessHasAFalseValue"></param>
  /// <returns></returns>
  /// <exception cref="Exception">if type is bool, and ResultValue is false</exception>
  public static Result<TReturn, TResultPayload> Success(
    TResultPayload value,
    string? message = null,
    bool isSuccessHasAFalseValue = false,
    string? fullTypeName = null
  )
  {
    if (typeof(TReturn) == typeof(bool) && value is bool == false)
    {
      if (isSuccessHasAFalseValue == false)
        throw new Exception("The value that is provided is boolean, and is provided as false - use override indicator to avoid exception");
    }

    return new Result<TReturn, TResultPayload>(
      isSuccess: true,
      value: value,
      messages: message != null ? [message] : null,
      fullTypeName: fullTypeName
    );
  }


  public static Result<TReturn, TResultPayload> Failure(
    params Exception[] exceptions
  ) =>
      new Result<TReturn, TResultPayload>(
        isSuccess: false,
        value: default,
        exceptions: exceptions
      );

  public static Result<TReturn, TResultPayload> Failure(
    params string[] messages
  ) =>
      new Result<TReturn, TResultPayload>(
        isSuccess: false,
        value: default,
        messages: messages
      );

  public static Result<TReturn, TResultPayload> Failure(
    IEnumerable<string> errorMessages
  ) =>
     new Result<TReturn, TResultPayload>(
       isSuccess: false,
       value: default,
       messages: errorMessages.ToArray()
     );

  public static Result<TReturn, TResultPayload> Failure(
    string errorMessage,
    Exception exception
  ) =>
      new Result<TReturn, TResultPayload>(
        isSuccess: false,
        value: default,
        messages: [errorMessage],
        exceptions: [exception]
      );

  public static Result<TReturn, TResultPayload> Failure<TInResultPayload, TInReturn>(Result<TInResultPayload, TInResultPayload> incomingResultFailure)
      where TInResultPayload : IResultPayload<TInResultPayload>
  {
    return new Result<TReturn, TResultPayload>(
        isSuccess: false,
        value: default,
        messages: [.. incomingResultFailure.Messages],
        exceptions: [.. incomingResultFailure.Exceptions]
    );
  }

  /// <summary>
  /// Access any Result<T>, and create new T Failure with the same messages and exceptions
  /// </summary>
  /// <param name="incomingResultFailure"></param>
  /// <returns></returns>
  public static Result<TReturn, TResultPayload> Failure(
    IEnumerable<string> messages,
    IEnumerable<Exception> exceptions
  )
  {
    return new Result<TReturn, TResultPayload>(
      isSuccess: false,
      value: default,
      messages: [.. messages],
      exceptions: [.. exceptions]
    );
  }

  public static Task<Result<TReturn, TResultPayload>> TestTaskResult()
  {
#if DEBUG
    return Task.FromResult(Result<TReturn, TResultPayload>.Success(value: default!));
#else
      throw new NotImplementedException(); // In release, this method should not be called!
#endif
  }

  ////public static Result<TOutput> TransformResult<TInput, TOutput>(this Result<TInput> incomingResult, Func<Result<TInput>, Result<TOutput>> transformFunc)
  //public static Result<TOutput> TransformResult<TInput, TOutput>(this TInput incomingResult, Func<TInput, TOutput> transformFunc)
  //  where TInput : Result<TInput>
  //  where TOutput : Result<TOutput>
  //{
  //  if (incomingResult is null)
  //    throw new ArgumentNullException(nameof(incomingResult), "Incoming Result cannot be null");
  //  if (transformFunc is null)
  //    throw new ArgumentNullException(nameof(transformFunc), "Transform function cannot be null");

  //  if (incomingResult.IsSuccessful)
  //  {
  //    var outputValue = transformFunc(incomingResult);
  //    return Result<TOutput>.Success(outputValue.ResultValue!, message: incomingResult.Messages.FlattenList());
  //  }
  //  return Result<TOutput>.Failure(incomingResult);

  //}

}
#endregion