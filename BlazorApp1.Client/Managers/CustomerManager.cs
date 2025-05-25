using BlazorApp1.Client.ResourceAccess.Interfaces;

namespace BlazorApp1.Client.Managers;

public class CustomerManager(CustomerInfoEngine customerEngine, ILogger<CustomerManager> logger)
{
  private readonly CustomerInfoEngine _customerEngine = customerEngine ?? throw new ArgumentNullException(nameof(customerEngine));
  private readonly ILogger<CustomerManager> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

  /// <summary>
  /// Returns JSON string of the Customer DTO - version depends on what type was asked for.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="id"></param>
  /// <returns></returns>
  /// <exception cref="InvalidOperationException"></exception>
  public Result<ICustomerDTO, ResultPayloadOfType<ICustomerDTO>> GetCustomer<TCustomerVersion>(string id) where TCustomerVersion : class, ICustomerDTO, new()
  {

#if DEBUG
    var typeName = typeof(TCustomerVersion).FullName ?? string.Empty;
#endif

    var result = customerEngine.GetCustomer<TCustomerVersion>(id) ?? throw new InvalidOperationException($"Customer not found for id {id}");
    if (result.Failed)
    {
      _logger.LogError(string.Join("|", result.Messages));
    } 

    return result;

  }

  public Result<IEnumerable<ICustomerDTO>,ResultEnumerablePayload<ICustomerDTO>> GetCustomers<TCustomerVersion>() 
    where TCustomerVersion : class, ICustomerDTO, new()
  {
    var customersDTO = customerEngine.GetCustomers<TCustomerVersion>() ?? throw new InvalidOperationException($"No customers found for version {typeof(TCustomerVersion).FullName}");
    if (customersDTO.Failed)
    {
      _logger.LogError(string.Join("|", customersDTO.Messages));
    } 

    return customersDTO;
  }
}


