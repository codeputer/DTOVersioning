namespace BlazorApp1.Client.Engines.Interfaces;

public interface ICustomerEngine
{
  Type CustomerType { get; }

  Result<ICustomerDTO, ResultPayloadOfType<CustomerV3>> GetCustomer(string id);

  Result<IEnumerable<ICustomerDTO>, ResultEnumerablePayload<ICustomerDTO>> GetCustomers();
}

