namespace BlazorApp1.Client.Engines;

public class CustomerEngineV3(CustomerRA customerRA) : ICustomerEngine
{
  private readonly CustomerRA _CustomerRA = customerRA ?? throw new ArgumentNullException(nameof(customerRA));

  public Type CustomerType => typeof(CustomerV3);

  public Result<IEnumerable<ICustomerDTO>> GetCustomer(string id)
  {
    return _CustomerRA.GetCustomer<CustomerV3>(id);
  }
}




