namespace BlazorApp1.Client.ResourceAccess;

public class CustomerRA_V2(ILogger<CustomerRA_V2> logger) : ICustomerRA<ICustomerDTO>
{
  private readonly ILogger<CustomerRA_V2> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

  public string CustomerVersionDTOType { get; private set; } = typeof(CustomerV2).FullName!;

  public Result<ICustomerDTO, ResultPayloadOfType<ICustomerDTO>> GetCustomer(string id)
  {
    CustomerVersionDTOType = typeof(CustomerV2).FullName!;

    var customerV2 = new CustomerV2
    {
      Id = id,
      FirstName = $"John_{id}",
      LastName = "Doe",
      Email = $"John_{id}@example.com"
    };

    var payload = ResultPayloadOfType<ICustomerDTO>.CreateInstance(customerV2);

    return Result<ICustomerDTO, ResultPayloadOfType<ICustomerDTO>>.Success(payload);

  }

  public Result<IEnumerable<ICustomerDTO>, ResultEnumerablePayload<ICustomerDTO>> GetCustomers()
  {
    CustomerVersionDTOType = typeof(IEnumerable<CustomerV2>).FullName!;

    List<ICustomerDTO> customers = [];
    for (int i = 1; i <= 5; i++)
    {
      var result = GetCustomer(i.ToString());

      if (result.IsSuccessful)
        customers.Add(result!.PayloadWrapper!.Payload);
      else
      {
        return Result<IEnumerable<ICustomerDTO>, ResultEnumerablePayload<ICustomerDTO>>.Failure(result.Messages);
      }
    }

    var payload = ResultEnumerablePayload<ICustomerDTO>.CreateInstance(customers);

    return Result<IEnumerable<ICustomerDTO>, ResultEnumerablePayload<ICustomerDTO>>.Success(payload);
  }
}


