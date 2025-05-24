
using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Client.ResourceAccess;

public class CustomerRA_V1(ILogger<CustomerRA_V1> logger) : ICustomerRA<ICustomerDTO>
{
  private readonly ILogger<CustomerRA_V1> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

  public string CustomerVersionDTOType { get; private set; }

  public ICustomerDTO GetCustomer<TDTOVersion>(string id) where TDTOVersion : ICustomerDTO
  {
    CustomerVersionDTOType = typeof(TDTOVersion).FullName!;
    return new CustomerV1
    {
      Id = id,
      FirstName = $"John_{id}",
      LastName = "Doe",
      Email = $"John_{id}@example.com"
    };
  }

  public IEnumerable<ICustomerDTO> GetCustomers<TDTOVersion>() where TDTOVersion : ICustomerDTO
  {
    CustomerVersionDTOType = typeof(IEnumerable<ICustomerDTO>).FullName!;

    List<ICustomerDTO> customers = new();
    for (int i = 1; i <= 5; i++)
    {
      customers.Add(GetCustomer<TDTOVersion>(i.ToString()));
    }
    return customers;
  }
}


