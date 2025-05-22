using BlazorApp1.Client.ResourceAccess.Interfaces;

namespace BlazorApp1.Client.Managers;

public class CustomerManager(IEnumerable<ICustomerRA<ICustomerDTO>> customerEngines, ILogger<CustomerManager> logger)
{
  private readonly IEnumerable<ICustomerRA<ICustomerDTO>> _customerEngines = customerEngines ?? throw new ArgumentNullException(nameof(customerEngines));
  private readonly ILogger<CustomerManager> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

  /// <summary>
  /// Returns JSON string of the Customer DTO - version depends on what type was asked for.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="id"></param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  public ICustomerDTO? GetCustomer<TCustomerVersion>(string id) where TCustomerVersion : class, ICustomerDTO, new()
  {

#if DEBUG
    var typeName = typeof(TCustomerVersion).FullName ?? string.Empty;
#endif

    //find the right engine version
    var customerEngine = _customerEngines.FirstOrDefault(pCustomerEngine => pCustomerEngine.CustomerVersionDTOType == typeof(TCustomerVersion).FullName);

    if (customerEngine == null)
    {
      _logger.LogError($"Customer engine not found for type {typeof(TCustomerVersion).Name}");
      throw new InvalidOperationException($"Customer engine not found for type {typeof(TCustomerVersion).Name}");
    }

    var customerVersionDTO =  customerEngine.GetCustomer<TCustomerVersion>(id) ?? throw new InvalidOperationException($"Customer not found for id {id}");

    return customerVersionDTO;

  }


}


