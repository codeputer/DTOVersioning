namespace BlazorApp1.Client.Engines;

public class CustomerEngineV1(CustomerEngine customerRA) : ICustomerEngine
{
  private readonly CustomerEngine _CustomerRA = customerRA ?? throw new ArgumentNullException(nameof(customerRA));

  public Type CustomerType { get; private set; } = default!;

  public ICustomerDTO? GetCustomer(string id)
  {
    CustomerType = typeof(CustomerV1);

    if (string.IsNullOrWhiteSpace(id))
      throw new ArgumentNullException(nameof(id), "Id cannot be null or empty");

    return  _CustomerRA.GetCustomer<CustomerV1>(id);
  }

  public IEnumerable<ICustomerDTO> GetCustomers()
  {
    CustomerType = typeof(IEnumerable<CustomerV1>); 

    return _CustomerRA.GetCustomers<CustomerV1>();
  } 
}


