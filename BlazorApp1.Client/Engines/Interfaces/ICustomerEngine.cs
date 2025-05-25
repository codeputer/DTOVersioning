namespace BlazorApp1.Client.Engines.Interfaces;

public interface ICustomerEngine
{
  Type CustomerType { get; }

  Result<bool, ResultBoolPayload> UpdateCustomer(ICustomerDTO newCustomerIN);
}

