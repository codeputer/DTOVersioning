
namespace BlazorApp1.Client.ResourceAccess;

public class CustomerRA_V2(ILogger<CustomerRA_V2> logger) : ICustomerRA<ICustomerDTO>
{
  private readonly ILogger<CustomerRA_V2> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

  public string CustomerVersionDTOType { get; private set; } = typeof(CustomerV2).FullName!;

  public ICustomerDTO GetCustomer(string id)
  {
    CustomerVersionDTOType = typeof(CustomerV2).FullName!;

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

  public IEnumerable<ICustomerDTO> GetCustomers()
  {
    CustomerVersionDTOType = typeof(IEnumerable<ICustomerDTO>).FullName!;

    List<ICustomerDTO> customers = [];
    for (int i = 1; i <= 5; i++)
    {
      customers.Add(GetCustomer(i.ToString()));
    }
    return customers;
  }

}

