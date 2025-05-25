namespace BlazorApp1.Client.ResourceAccess;

public class CustomerRA_V3(ILogger<CustomerRA_V3> logger) : ICustomerRA<ICustomerDTO>
{
  private readonly ILogger<CustomerRA_V3> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

  public string CustomerVersionDTOType { get; private set; } = typeof(CustomerV3).FullName!;

  public Result<ICustomerDTO, ResultPayloadOfType<ICustomerDTO>> GetCustomer(string id)
  {
    CustomerVersionDTOType = typeof(CustomerV3).FullName!;

    // Simulating a scenario where the customer is not found

    var msg = $"CustomerRA_V3 with ID:{id} FAILED.";

    return Result<ICustomerDTO, ResultPayloadOfType<ICustomerDTO>>.Failure(msg);

  }

  public Result<IEnumerable<ICustomerDTO>, ResultEnumerablePayload<ICustomerDTO>> GetCustomers()
  {
    CustomerVersionDTOType = typeof(IEnumerable<CustomerV3>).FullName!;

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


