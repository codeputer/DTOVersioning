namespace BlazorApp1.Client.ResourceAccess;

public class CustomerRA_V1(ILogger<CustomerRA_V1> logger) : ICustomerRA<ICustomerDTO>
{
  private readonly ILogger<CustomerRA_V1> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

  public string CustomerVersionDTOType { get; private set; } = typeof(CustomerV1).FullName!;

  public ICustomerDTO GetCustomer(string id)
  {
    CustomerVersionDTOType = typeof(CustomerV1).FullName!;

    return new CustomerV1
    {
      Id = id,
      FirstName = $"John_{id}",
      LastName = "Doe",
      Email = $"John_{id}@example.com"
    };
  }

  public IEnumerable<ICustomerDTO> GetCustomers()
  {
    CustomerVersionDTOType = typeof(IEnumerable<CustomerV1>).FullName!;

    List<ICustomerDTO> customers = new List<ICustomerDTO>();
    for (int i = 1; i <= 5; i++)
    {
      customers.Add(GetCustomer(i.ToString()));
    }
    return customers;
  }
}


