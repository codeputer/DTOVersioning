namespace BlazorApp1.Client.Engines.Interfaces;

public interface ICustomerEngine
{
  Type CustomerType { get; }

  IEnumerable<T> GetCustomer<T>(string id) where T : class, ICustomerDTO, new();
}