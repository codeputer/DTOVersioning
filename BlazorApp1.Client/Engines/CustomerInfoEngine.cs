using System.Net.WebSockets;

namespace BlazorApp1.Client.Engines;

public class CustomerInfoEngine(IEnumerable<ICustomerRA<ICustomerDTO>> customerRAs, ILogger<CustomerInfoEngine> logger) 
{
  private readonly IEnumerable<ICustomerRA<ICustomerDTO>> _customerRAs = customerRAs ?? throw new ArgumentNullException(nameof(customerRAs));
  private readonly ILogger<CustomerInfoEngine> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

  public string CustomerVersionDTOType => typeof(CustomerInfoEngine).FullName!;

  public Result<ICustomerDTO, ResultPayloadOfType<ICustomerDTO>> GetCustomer<TDTOVersion>(string id) where TDTOVersion : class , ICustomerDTO
  {
    if (string.IsNullOrWhiteSpace(id))
      throw new ArgumentNullException(nameof(id), "Id cannot be null or empty");

    var versionOfDTO = typeof(TDTOVersion);
    var raRequired = _customerRAs.FirstOrDefault(pCustomerRA => pCustomerRA.CustomerVersionDTOType == versionOfDTO.FullName);
    if (raRequired == null)
    {
      _logger.LogError($"No customer resource access found for {versionOfDTO.FullName}");
      throw new InvalidOperationException($"No customer resource access found for {versionOfDTO.FullName}");
    }

    var result = raRequired.GetCustomer(id);

    if (result.Failed)
    {
      _logger.LogError(string.Join("|", result.Messages));
      return Result<ICustomerDTO, ResultPayloadOfType<ICustomerDTO>>.Failure(result.Messages);
    }

    return result;
  }

  public Result<IEnumerable<ICustomerDTO>, ResultEnumerablePayload<ICustomerDTO>> GetCustomers<TDTOVersion>()
    where TDTOVersion : ICustomerDTO
  {
    var versionOfDTO = typeof(TDTOVersion);
    var raRequired = _customerRAs.FirstOrDefault(pCustomerRA => pCustomerRA.CustomerVersionDTOType == versionOfDTO.FullName);
    if (raRequired == null)
    {
      _logger.LogError($"No customer resource access found for {versionOfDTO.FullName}");
      throw new InvalidOperationException($"No customer resource access found for {versionOfDTO.FullName}");
    }

    var result = raRequired.GetCustomers();
    if (result.Failed) {  
      _logger.LogError(string.Join("|", result.Messages));
      return Result<IEnumerable<ICustomerDTO>, ResultEnumerablePayload<ICustomerDTO>>.Failure(result.Messages);
    }

    return result;

  }
}
