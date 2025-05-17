namespace BlazorApp1.Client.Managers;

public class CustomerManager(IEnumerable<ICustomerEngine> customerEngines, ILogger<CustomerManager> logger)
{
  private readonly IEnumerable<ICustomerEngine> _customerEngines = customerEngines ?? throw new ArgumentNullException(nameof(customerEngines));
  private readonly ILogger<CustomerManager> _logger = logger ?? throw new ArgumentNullException(nameof(logger));


  /// <summary>
  /// Returns JSON string of the Customer DTO - version depends on what type was asked for.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="id"></param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  public string GetCustomer<T>(string id) where T : class, ICustomerDTO, new()
  {

#if DEBUG
    var typeName = typeof(T).FullName ?? string.Empty;
#endif

    //find the right engine version
    var customerEngine = _customerEngines.FirstOrDefault(pCustomerEngine => pCustomerEngine.CustomerType == typeof(T)); 

    if (customerEngine == null)
    {
      _logger.LogError($"Customer engine not found for type {typeof(T).Name}");
      throw new InvalidOperationException($"Customer engine not found for type {typeof(T).Name}");
    }

    var resultEnumeration =  customerEngine.GetCustomer(id) ?? throw new InvalidOperationException($"Customer not found for id {id}");

    return JsonSerializer.Serialize(resultEnumeration, new JsonSerializerOptions
    {
      WriteIndented = true,
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
      DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    });

  }


}


