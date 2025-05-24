using BlazorApp1.Client.Engines.Interfaces;

namespace BlazorApp1.Client.Engines;

public class CustomerEngineV3(CustomerEngine customerRA) : ICustomerEngine
{
  private readonly CustomerEngine _CustomerRA = customerRA ?? throw new ArgumentNullException(nameof(customerRA));

  public Type CustomerType { get; private set; } = default!;

  public Result<ICustomerDTO, ResultPayloadOfType<CustomerV3>> GetCustomer(string id)
  {
    CustomerType = typeof(CustomerV3);

    if (string.IsNullOrWhiteSpace(id))
      throw new ArgumentNullException(nameof(id), "Id cannot be null or empty");

    var customerDTO = _CustomerRA.GetCustomer<CustomerV3>(id) as CustomerV3
      ?? throw new InvalidOperationException($"Customer not found for id {id}"); 

    var resultOfPayload = ResultPayloadOfType<CustomerV3>.CreateInstance(customerDTO);

    return Result<ICustomerDTO, ResultPayloadOfType<CustomerV3>>.Success(resultOfPayload);
  }

  public Result<IEnumerable<ICustomerDTO>, ResultEnumerablePayload<CustomerV3>> GetCustomers()
  {
    CustomerType = typeof(IEnumerable<CustomerV3>);

    var customers = _CustomerRA.GetCustomers<CustomerV3>() as IEnumerable<CustomerV3>
      ?? throw new InvalidOperationException("No customers found.");

    var resultofPayload = ResultEnumerablePayload<CustomerV3>.CreateInstance(customers);

    return Result<IEnumerable<ICustomerDTO>, ResultEnumerablePayload<CustomerV3>>.Success(resultofPayload);
  }

  Result<IEnumerable<ICustomerDTO>, ResultEnumerablePayload<ICustomerDTO>> ICustomerEngine.GetCustomers()
  {
    CustomerType = typeof(IEnumerable<CustomerV3>);

    var customers = _CustomerRA.GetCustomers<CustomerV3>() as IEnumerable<CustomerV3>
      ?? throw new InvalidOperationException("No customers found.");

    var resultofPayload = ResultEnumerablePayload<ICustomerDTO>.CreateInstance(customers);

    return Result<IEnumerable<ICustomerDTO>, ResultEnumerablePayload<ICustomerDTO>>.Success(resultofPayload);
  }
}




