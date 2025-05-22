
namespace BlazorApp1.Client.ResourceAccess;

public class CustomerRA_V3(ILogger<CustomerRA_V1> logger) : ICustomerRA<ICustomerDTO>
{
  private readonly ILogger<CustomerRA_V1> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

  public string CustomerVersionDTOType => typeof(CustomerV3).FullName!;

  public ICustomerDTO GetCustomer<TDTOVersion>(string id) where TDTOVersion : ICustomerDTO
  {
    return new CustomerV3
    {
      Id = id,
      FirstName = $"John_{id}",
      LastName = "Doe",
      Email = $"John_{id}@example.com",
      Address = $"123 Main St_{id}",
      Citizen = "Canadian",
      Heritage = "Dutch",
    };
  }

  public List<ICustomerDTO> GetCustomers<TDTOVersion>() where TDTOVersion : ICustomerDTO
  {
    List<ICustomerDTO> customers = new();
    for (int i = 1; i <= 5; i++)
    {
      customers.Add(GetCustomer<TDTOVersion>(i.ToString()));
    }
    return customers;
  }
}


