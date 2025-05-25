namespace BlazorApp1.Client.Engines;

public class CustomerInfoEngine(IEnumerable<ICustomerRA<ICustomerDTO>> customerRAs, ILogger<CustomerInfoEngine> logger) 
{
  private readonly IEnumerable<ICustomerRA<ICustomerDTO>> _customerRAs = customerRAs ?? throw new ArgumentNullException(nameof(customerRAs));
  private readonly ILogger<CustomerInfoEngine> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

  public string CustomerVersionDTOType => typeof(CustomerInfoEngine).FullName!;

  public ICustomerDTO GetCustomer<TDTOVersion>(string id) where TDTOVersion : ICustomerDTO
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

    return raRequired.GetCustomer(id);
  }

  public IEnumerable<ICustomerDTO> GetCustomers<TDTOVersion>() where TDTOVersion : ICustomerDTO
  {
     var versionOfDTO = typeof(TDTOVersion);
    var raRequired = _customerRAs.FirstOrDefault(pCustomerRA => pCustomerRA.CustomerVersionDTOType == versionOfDTO.FullName);
    if (raRequired == null)
    {
      _logger.LogError($"No customer resource access found for {versionOfDTO.FullName}");
      throw new InvalidOperationException($"No customer resource access found for {versionOfDTO.FullName}");
    }

    return raRequired.GetCustomers();

  }
}
