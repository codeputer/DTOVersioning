namespace BlazorApp1.Client.Utilities;

public class Result<TReturn>
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
  [JsonPropertyName(nameof(ResultValue))]
  public TReturn? ResultValue { get; }

  [JsonPropertyName(nameof(Messages))]
  public List<string> Messages { get; set; } = []; //must have setter for deserialization

  [JsonPropertyName(nameof(Exceptions))]
  public List<Exception> Exceptions { get; set; } = []; //must have setter for deserialization

  private Result(bool isSuccess, TReturn? value, string[]? messages = null, Exception[]? exceptions = null, string? fullTypeName = null)
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
      Messages.Add("Error: Result was a failure but no error messages were provided. A failure should always have error messages");
    }

    if (ResultStatus && ResultValue is null)
    {
#if DEBUG
      // In debug mode, throw an exception if ResultStatus is true but ResultValue is null
      System.Diagnostics.Debug.Assert(IsSuccessful == false, "ResultStatus is true but ResultValue is null. This should not happen.");
#endif
      Messages.Add("Warning!! Result is True, but the ResultValue is null - this is an Assertion on the Result class as Success should always return a not null result.");
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

  /// <summary>
  /// Checks the number of changes that have occurred, if greater than zero, then ResultStatus is true, otherwise false
  /// </summary>
  /// <param name="numberOfChanges"></param>
  /// <param name="failureMessage"></param>
  /// <returns></returns>
  public static Result<bool> SaveChangesActivity(int numberOfChanges, string failureMessage = "change tracking detected no changes")
  {
    return Result<bool>.CheckCondition(() => numberOfChanges > 0, failureMessage);
  }

  /// <summary>
  /// Using the condition, will determine if Result.Success is provided, or Result.Failure is provided
  /// </summary>
  /// <param name="validationTest"></param>
  /// <param name="messages"></param>
  /// <returns></returns>
  public static Result<bool> CheckCondition(Expression<Func<bool>> validationTest, params string[] messages)
  {
    Func<bool> compiledExpression = validationTest.Compile();
    bool result = compiledExpression();
    if (result)
      return Result<bool>.Success(true);
    else
      return Result<bool>.Failure(messages);
  }


  /// <summary>
  /// The condition is checked, and if the condition is true, then the value is returned as a success
  /// 
  /// var result = ResultHelper.CheckCondition(name, () => name.Length > 5, "Failure message");
  /// </summary>
  /// <param name="value">if condition is successful, value should not be null</param>
  /// <param name="validationTest"></param>
  /// <param name="messages"></param>
  /// <returns></returns>
  public static Result<TReturn> CheckCondition(TReturn? value, Expression<Func<bool>> validationTest, params string[] messages)
  {
    if (validationTest.Compile()()) // Compile() first then Execute the function () - reason for ()()
    {
      if (value == null)
        throw new ArgumentNullException(nameof(value), "Value should not be null if the condition is successful");

      return Result<TReturn>.Success(value!);
    }

    // Extract text representation of the expression
    string conditionText = validationTest.Body.ToString();
    string defaultMessage = $"Condition failed: {conditionText}";

    return Result<TReturn>.Failure(messages.Length > 0 ? messages : [defaultMessage]);
  }

  /// <summary>
  /// Success is an indication that the return value was a successful operation, and the value is provided in the generic type provided
  /// However, if the type is bool, and the resulting value is false - is it a successful operation?
  /// The override must be use if the ResultValue is true, and the ResultValue is false.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="message"></param>
  /// <param name="isSuccessHasAFalseValue"></param>
  /// <returns></returns>
  /// <exception cref="Exception">if type is bool, and ResultValue is false</exception>
  public static Result<TReturn> Success(TReturn value, string? message = null, bool isSuccessHasAFalseValue = false, string? fullTypeName = null)
  {
    if (typeof(TReturn) == typeof(bool) && value is bool == false)
    {
      if (isSuccessHasAFalseValue == false)
        throw new Exception("The value that is provided is boolean, and is provided as false - use override indicator to avoid exception");
    }

    return new(true, value, message != null ? [message] : null, fullTypeName: fullTypeName);
  }

  public static Result<bool> Success(ICustomerDTO customerDTO) =>
    new(true, true);

  public static Result<TReturn> Failure(params Exception[] exceptions) =>
      new(false, default, null, exceptions);

  public static Result<TReturn> Failure(params string[] messages) =>
      new(false, default, messages);

  public static Result<TReturn> Failure(IEnumerable<string> errorMessages) =>
     new(false, default, errorMessages.ToArray());

  public static Result<TReturn> Failure(string errorMessage, Exception exception) =>
      new(false, default, [errorMessage], [exception]);

  public static Result<TReturn> Failure<TIn>(Result<TIn> incomingResultFailure) =>
      new(false, default, [.. incomingResultFailure.Messages], [.. incomingResultFailure.Exceptions]);

  /// <summary>
  /// Access any Result<T>, and create new T Failure with the same messages and exceptions
  /// </summary>
  /// <param name="incomingResultFailure"></param>
  /// <returns></returns>
  public static Result<TReturn> Failure(IEnumerable<string> messages, IEnumerable<Exception> exceptions)
  {
    return new(false, default, [.. messages], [.. exceptions]);
  }

  // Update the TestTaskResult method to return the correct type
  public static Task<Result<TReturn>> TestTaskResult()
  {
#if DEBUG
    return Task.FromResult(Result<TReturn>.Success(default(TReturn)!));
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
  //    throw new ArgumentNullException(nameof(incomingResult), "Incoming result cannot be null");
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
