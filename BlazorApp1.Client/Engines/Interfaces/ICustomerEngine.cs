namespace BlazorApp1.Client.Engines.Interfaces;

public interface ICustomerEngine
{
  Type CustomerType { get; }

  Result<ICustomerDTO, ResultPayloadOfType<ICustomerDTO>> UpdateCustomer(ICustomerDTO newCustomerIN);
}

