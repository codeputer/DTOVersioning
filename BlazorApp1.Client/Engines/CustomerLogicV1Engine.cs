namespace BlazorApp1.Client.Engines;

public class CustomerLogicV1Engine(IEnumerable<ICustomerRA<ICustomerDTO>> customerRAs, ILogger<CustomerLogicV1Engine> logger) : ICustomerEngine
{
  ILogger<CustomerLogicV1Engine> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  private readonly IEnumerable<ICustomerRA<ICustomerDTO>> _customerRAs = customerRAs ?? throw new ArgumentNullException(nameof(customerRAs));

  public Type CustomerType => typeof(CustomerV1);

  public Result<ICustomerDTO, ResultPayloadOfType<ICustomerDTO>> UpdateCustomer(ICustomerDTO newCustomerIN)
  {
    var _customerRA = _customerRAs.FirstOrDefault(pCustomerRA=> pCustomerRA.CustomerVersionDTOType == CustomerType.Name)
      ?? throw new InvalidOperationException($"No customer RA found for type {CustomerType.Name}");

    //logic goes here, for example, validate the customer data
    if (newCustomerIN is not CustomerV1 newCustomer)
    {
      var msg = $"Invalid customer type provided for update: {newCustomerIN.GetType().FullName}";
      _logger.LogError(msg);
      return Result<ICustomerDTO, ResultPayloadOfType<ICustomerDTO>>.Failure(msg);
    }

    var payload = ResultPayloadOfType<ICustomerDTO>.CreateInstance(newCustomer);

    return Result<ICustomerDTO, ResultPayloadOfType<ICustomerDTO>>.Success(payload);
  }
}
