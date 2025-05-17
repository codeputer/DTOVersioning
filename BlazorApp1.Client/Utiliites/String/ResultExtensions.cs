namespace BlazorApp1.Client.Utilities.String;

public static class ResultExtensions
{
    private static readonly JsonSerializerOptions jsonResultDeserializor = new()
    {
        Converters =
            {
                new ResultJsonDeserializor()
            }
    };

    /// <summary>
    /// Deserialize the HttpResponseMessage to a Result<TInterface> where TInterface has been implemented by TConcrete
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <typeparam name="TConcrete"></typeparam>
    /// <param name="responseMessage"></param>
    /// <param name="methodCalling"></param>
    /// <returns></returns>
    public static async Task<Result<TInterface>> DeserializeResultAsync<TInterface, TConcrete>(this HttpResponseMessage responseMessage, [CallerMemberName] string? methodCalling = null)
      where TConcrete : class, TInterface, new()
    {
        methodCalling ??= "Unknown";
#if DEBUG
        var TInterfaceName = typeof(TInterface).FullName;
        var TConcreteName = typeof(TConcrete).FullName;
#endif

        if (responseMessage.IsSuccessStatusCode == false)
        {
            return Result<TInterface>.Failure($"HTTP Request to fetch:{methodCalling} has failed with Response Code:{responseMessage.StatusCode}");
        }

        if (typeof(TConcrete).IsValueType)
        {
            return Result<TInterface>.Failure($"TConcrete must be of type Class!");
        }

        if (typeof(TInterface).IsInterface == false)
        {
            return Result<TInterface>.Failure($"TInterface must be of type Interface!");
        }

        var deserializeToConcrete = DeserializeResult<TConcrete>(await responseMessage.Content.ReadAsStringAsync(), typeof(TInterface));

        if (deserializeToConcrete is null)
        {
            return Result<TInterface>.Failure($"Deserialization return a null result for:{typeof(TInterface).FullName.ParamMarkers()} ");
        }

        if (deserializeToConcrete.Failed)
        {
            return Result<TInterface>.Failure(deserializeToConcrete.Messages, deserializeToConcrete.Exceptions);
        }

        if (deserializeToConcrete.ResultValue is TInterface resultValue)
        {
            return Result<TInterface>.Success(resultValue);
        }
        else
        {
            return Result<TInterface>.Failure($"Deserialized of {typeof(TInterface).FullName.ParamMarkers()} cannot be cast to Interface:{typeof(TInterface).FullName.ParamMarkers()}");
        }
    }


    /// <summary>
    /// Deserialize the HttpResponseMessage to a Result<T>
    /// </summary>
    /// <typeparam name="TConcrete"></typeparam>
    /// <param name="responseMessage"></param>
    /// <param name="methodCalling"></param>
    /// <returns></returns>
    public static async Task<Result<TConcrete>> DeserializeResultAsync<TConcrete>(this HttpResponseMessage responseMessage, [CallerMemberName] string? methodCalling = null)
    {
        methodCalling ??= "Unknown";
        if (responseMessage.IsSuccessStatusCode == false)
        {
            return Result<TConcrete>.Failure($"HTTP Request to fetch:{methodCalling} has failed with Response Code:{responseMessage.StatusCode}");
        }

        return DeserializeResult<TConcrete>(await responseMessage.Content.ReadAsStringAsync());
    }

    public static Result<TConcrete> DeserializeResult<TConcrete>(string json, Type? TInterface = null)
    {
        if (typeof(TConcrete).IsInterface)
        {
            throw new Exception($"Type of T:{typeof(TConcrete).FullName.ParamMarkers()} is an Interface, and cannot be instantiated");
        }

        if (TInterface is not null && TInterface.IsInterface == false)
        {
            throw new Exception($"Type of TInterface:{TInterface.FullName.ParamMarkers()} is not an Interface");
        }

        //instantiate using Result<object> first, and then convert to the type specified
        Result<object>? deserializedResult = JsonSerializer.Deserialize<Result<object>>(json, jsonResultDeserializor);
        if (deserializedResult is null)
        {
            return Result<TConcrete>.Failure($"Failed to de-serialize json to Result<T> where type is: {typeof(TConcrete).FullName.ParamMarkers()}");
        }

        if (deserializedResult.Failed)
        {
            return Result<TConcrete>.Failure(deserializedResult.Messages, deserializedResult.Exceptions);
        }

        //validate that the type in the JSON matches the type which was used to serialize the payload
        var typeFullNameCheck = TInterface?.FullName ?? typeof(TConcrete).FullName;
        if (typeFullNameCheck != deserializedResult.FullTypeName)
        {
            return Result<TConcrete>.Failure($"Input type of T:{typeFullNameCheck.ParamMarkers()} does NOT match type in JSON:{deserializedResult.FullTypeName.ParamMarkers()}");
        }

        TConcrete? resultValue;
        //convert to the specified type
        try
        {
            //NOTE: if the rows appear but not the properties, ensure the DTO has corrected the property names during serialization.
            resultValue = (TConcrete)Convert.ChangeType(deserializedResult.ResultValue!, typeof(TConcrete));
        }
        catch
        {
            return Result<TConcrete>.Failure("Failed to convert deserialized result to specified type caused an Exception");
        }

        if (resultValue is not null)
        {
            return Result<TConcrete>.Success(resultValue);
        }

        return Result<TConcrete>.Failure("resultValue in payload may be null?");
    }


    public static string FlattenList(this IEnumerable<string> messages, string delimiter = " | ")
    {
        var messageCount = messages.Count();

        if (messageCount == 1 || messageCount == 0)
            return messages.FirstOrDefault() ?? string.Empty;


        return string.Join(delimiter, messages);

    }

    /// <summary>
    /// Transform Result<IEnumerable<TInput>> to Result<IEnumerable<TOutput>>
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="inputResult"></param>
    /// <param name="transformFunc"></param>
    /// <returns></returns>
    public static Result<TOutput> TransformResult<TIncomingResult, TOutput>(this Result<TIncomingResult> incomingResult, Func<TIncomingResult, TOutput> transformFunc)
    {
        if (incomingResult.IsSuccessful)
        {
            var outputValue = transformFunc(incomingResult.ResultValue!);
            string? successMessage = incomingResult.Messages.FlattenList();

            return Result<TOutput>.Success(outputValue, message: successMessage);
        }

        var failureResult = Result<TOutput>.Failure(incomingResult.Messages, incomingResult.Exceptions);

        return failureResult;

    }


}
