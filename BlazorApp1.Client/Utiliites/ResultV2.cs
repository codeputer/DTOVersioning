namespace BlazorApp1.Client.Utilities;

public class ResultV2<TReturn>
{

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
  public bool IsSuccessful => ResultStatus == true && ResultValue is not null;

  /// <summary>
  /// Failed is a ResultStatus==false, and the ResultValue may be null - check Messages or Exceptions for more information
  /// </summary>
  [JsonIgnore]
  public bool Failed => ResultStatus == false;

  /// <summary>
  /// Reflects the value of type T when method call returned a ResultStatus==true or Successful()
  /// </summary>
  [JsonIgnore]
  public TReturn? ResultValue { get; private set; }

  string? resultJson;
  [JsonPropertyName(nameof(ResultJson))]
  public string? ResultJson
  {  
    get
    {
      resultJson = ResultValue is null ? null : JsonSerializer.Serialize(ResultValue);
      return resultJson;  
    }
    set
    {
      if (value is not null)
      {
        try
        {
          ResultValue = JsonSerializer.Deserialize<TReturn>(value);
        }
        catch (JsonException ex)
        {
          Exceptions.Add(ex);
          Messages.Add($"Error deserializing ResultJson: {ex.Message}");
        }
      }
    }
  }

  [JsonPropertyName(nameof(Messages))]
  public List<string> Messages { get; set; } = []; //must have setter for deserialization

  [JsonPropertyName(nameof(Exceptions))]
  public List<Exception> Exceptions { get; set; } = []; //must have setter for deserialization

  [JsonConstructor]
  public ResultV2(
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

  //Added internal constructor to allow for internal use only and differenciate fro JsonConstructor
  private ResultV2(
    bool internalConstructor,
    bool isSuccess,
    TReturn? value,
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
    ResultValue = value;

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
      Messages.Add("Error: ResultV2 was a failure but no error messages were provided. A failure should always have error messages");
    }

    if (ResultStatus && ResultValue is null)
    {
#if DEBUG
      // In debug mode, throw an exception if ResultStatus is true but ResultValue is null
      System.Diagnostics.Debug.Assert(IsSuccessful == false, "ResultStatus is true but ResultValue is null. This should not happen.");
#endif
      Messages.Add("Warning!! ResultV2 is True, but the ResultValue is null - this is an Assertion on the ResultV2 class as Success should always return a not null ResultV2.");
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
  /// <summary>
  /// Checks the number of changes that have occurred, if greater than zero, then ResultStatus is true, otherwise false
  /// </summary>
  /// <param name="numberOfChanges"></param>
  /// <param name="failureMessage"></param>
  /// <returns></returns>
  public static ResultV2<bool> SaveChangesActivity(
    int numberOfChanges,
    string failureMessage = "change tracking detected no changes"
  )
  {
    return ResultV2<bool>.CheckCondition(
      validationTest: () => numberOfChanges > 0,
      messages: failureMessage
    );
  }

  /// <summary>
  /// Using the condition, will determine if ResultV2.Success is provided, or ResultV2.Failure is provided
  /// </summary>
  /// <param name="validationTest"></param>
  /// <param name="messages"></param>
  /// <returns></returns>
  public static ResultV2<bool> CheckCondition(
    Expression<Func<bool>> validationTest,
    params string[] messages
  )
  {
    Func<bool> compiledExpression = validationTest.Compile();
    bool ResultV2 = compiledExpression();
    if (ResultV2)
      return ResultV2<bool>.Success(value: true);
    else
      return ResultV2<bool>.Failure(messages: messages);
  }


  /// <summary>
  /// The condition is checked, and if the condition is true, then the value is returned as a success
  /// 
  /// var ResultV2 = ResultV2Helper.CheckCondition(name, () => name.Length > 5, "Failure message");
  /// </summary>
  /// <param name="value">if condition is successful, value should not be null</param>
  /// <param name="validationTest"></param>
  /// <param name="messages"></param>
  /// <returns></returns>
  public static ResultV2<TReturn> CheckCondition(
    TReturn? value,
    Expression<Func<bool>> validationTest,
    params string[] messages
  )
  {
    if (validationTest.Compile()()) // Compile() first then Execute the function () - reason for ()()
    {
      if (value == null)
        throw new ArgumentNullException(nameof(value), "Value should not be null if the condition is successful");

      return ResultV2<TReturn>.Success(value: value!);
    }

    // Extract text representation of the expression
    string conditionText = validationTest.Body.ToString();
    string defaultMessage = $"Condition failed: {conditionText}";

    return ResultV2<TReturn>.Failure(messages: messages.Length > 0 ? messages : [defaultMessage]);
  }

  /// <summary>
  /// Success is an indication that the return value was a successful operation, and the value is provided in the generic type provided
  /// However, if the type is bool, and the ResultV2ing value is false - is it a successful operation?
  /// The override must be use if the ResultValue is true, and the ResultValue is false.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="message"></param>
  /// <param name="isSuccessHasAFalseValue"></param>
  /// <returns></returns>
  /// <exception cref="Exception">if type is bool, and ResultValue is false</exception>
  public static ResultV2<TReturn> Success(
    TReturn value,
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

    return new ResultV2<TReturn>(
      internalConstructor: true,
      isSuccess: true,
      value: value,
      messages: message != null ? [message] : null,
      fullTypeName: fullTypeName
    );
  }

  public static ResultV2<bool> Success(
    ICustomerDTO customerDTO
  ) =>
    new ResultV2<bool>(
      internalConstructor: true,
      isSuccess: true,
      value: true
    );

  public static ResultV2<TReturn> Failure(
    params Exception[] exceptions
  ) =>
      new ResultV2<TReturn>(
        internalConstructor: true,
        isSuccess: false,
        value: default,
        exceptions: exceptions
      );

  public static ResultV2<TReturn> Failure(
    params string[] messages
  ) =>
      new ResultV2<TReturn>(
        internalConstructor: true,
        isSuccess: false,
        value: default,
        messages: messages
      );

  public static ResultV2<TReturn> Failure(
    IEnumerable<string> errorMessages
  ) =>
     new ResultV2<TReturn>(
       internalConstructor: true,
       isSuccess: false,
       value: default,
       messages: errorMessages.ToArray()
     );

  public static ResultV2<TReturn> Failure(
    string errorMessage,
    Exception exception
  ) =>
      new ResultV2<TReturn>(
        internalConstructor: true,
        isSuccess: false,
        value: default,
        messages: [errorMessage],
        exceptions: [exception]
      );

  public static ResultV2<TReturn> Failure<TIn>(
    ResultV2<TIn> incomingResultV2Failure
  ) =>
      new ResultV2<TReturn>(
        internalConstructor: true,
        isSuccess: false,
        value: default,
        messages: [.. incomingResultV2Failure.Messages],
        exceptions: [.. incomingResultV2Failure.Exceptions]
      );

  /// <summary>
  /// Access any ResultV2<T>, and create new T Failure with the same messages and exceptions
  /// </summary>
  /// <param name="incomingResultV2Failure"></param>
  /// <returns></returns>
  public static ResultV2<TReturn> Failure(
    IEnumerable<string> messages,
    IEnumerable<Exception> exceptions
  )
  {
    return new ResultV2<TReturn>(
      internalConstructor: true,
      isSuccess: false,
      value: default,
      messages: [.. messages],
      exceptions: [.. exceptions]
    );
  }

  public static Task<ResultV2<TReturn>> TestTaskResultV2()
  {
#if DEBUG
    return Task.FromResult(ResultV2<TReturn>.Success(value: default(TReturn)!));
#else
      throw new NotImplementedException(); // In release, this method should not be called!
#endif
  }

  ////public static ResultV2<TOutput> TransformResultV2<TInput, TOutput>(this ResultV2<TInput> incomingResultV2, Func<ResultV2<TInput>, ResultV2<TOutput>> transformFunc)
  //public static ResultV2<TOutput> TransformResultV2<TInput, TOutput>(this TInput incomingResultV2, Func<TInput, TOutput> transformFunc)
  //  where TInput : ResultV2<TInput>
  //  where TOutput : ResultV2<TOutput>
  //{
  //  if (incomingResultV2 is null)
  //    throw new ArgumentNullException(nameof(incomingResultV2), "Incoming ResultV2 cannot be null");
  //  if (transformFunc is null)
  //    throw new ArgumentNullException(nameof(transformFunc), "Transform function cannot be null");

  //  if (incomingResultV2.IsSuccessful)
  //  {
  //    var outputValue = transformFunc(incomingResultV2);
  //    return ResultV2<TOutput>.Success(outputValue.ResultValue!, message: incomingResultV2.Messages.FlattenList());
  //  }
  //  return ResultV2<TOutput>.Failure(incomingResultV2);

  //}

}
#endregion