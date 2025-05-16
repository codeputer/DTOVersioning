namespace BlazorApp1.Client.Engines;

public class CustomerEngineV2(CustomerRA customerRA) : ICustomerEngine
{
  private readonly CustomerRA _CustomerRA = customerRA ?? throw new ArgumentNullException(nameof(customerRA));
  
  public Type CustomerType => typeof(CustomerV2);

  public IEnumerable<T> GetCustomer<T>(string id) where T : class, ICustomerDTO, new()
  {
    // Call the non-generic GetCustomer and cast results to T
    return _CustomerRA.GetCustomer(typeof(T), id).OfType<T>();
  }
}


