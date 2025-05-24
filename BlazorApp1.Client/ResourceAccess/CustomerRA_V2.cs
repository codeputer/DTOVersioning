

using BlazorApp1.Client.ResourceAccess.Interfaces;

namespace BlazorApp1.Client.ResourceAccess;

public class CustomerRA_V2(ILogger<CustomerRA_V2> logger) : ICustomerRA<ICustomerDTO>
{
  private readonly ILogger<CustomerRA_V2> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

  public string CustomerVersionDTOType { get; private set; }

  public ICustomerDTO GetCustomer<TDTOVersion>(string id) where TDTOVersion : ICustomerDTO
  {
    CustomerVersionDTOType = typeof(TDTOVersion).FullName!;

    return new CustomerV2
    {
      Id = id,
      FirstName = $"John_{id}",
      LastName = "Doe",
      Email = $"John_{id}@example.com",
      Address = $"123 Main St_{id}",
      Citizen = "Canadian"
    };
  }

  public IEnumerable<ICustomerDTO> GetCustomers<TDTOVersion>() where TDTOVersion : ICustomerDTO
  {
    CustomerVersionDTOType = typeof(IEnumerable<ICustomerDTO>).FullName!;

    List<ICustomerDTO> customers = [];
    for (int i = 1; i <= 5; i++)
    {
      customers.Add(GetCustomer<TDTOVersion>(i.ToString()));
    }
    return customers;
  }

}

