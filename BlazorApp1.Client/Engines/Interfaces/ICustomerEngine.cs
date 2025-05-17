namespace BlazorApp1.Client.Engines.Interfaces;

public interface ICustomerEngine
{
  Type CustomerType { get; }

  Result<IEnumerable<ICustomerDTO>> GetCustomer(string id);

}