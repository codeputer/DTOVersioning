namespace BlazorApp1.Client.Engines;

public class CustomerEngineV1(CustomerEngine customerRA) : ICustomerEngine
{
  private readonly CustomerEngine _CustomerRA = customerRA ?? throw new ArgumentNullException(nameof(customerRA));

  public Type CustomerType => typeof(CustomerV1);

  public ICustomerDTO? GetCustomer(string id)
  {
    if (string.IsNullOrWhiteSpace(id))
      throw new ArgumentNullException(nameof(id), "Id cannot be null or empty");

    return  _CustomerRA.GetCustomer<CustomerV1>(id);
  }
}


