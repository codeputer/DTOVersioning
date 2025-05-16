using BlazorApp1.Client.Engines;
using BlazorApp1.Client.Engines.Interfaces;
using LateralDTOSample.ResourceAccess;

namespace BlazorApp1.Client.ResourceAccess;

public class CustomerRA
{
  public IEnumerable<ICustomerDTO> GetCustomer(Type customerType, string id)
  {
    return customerType.FullName switch
    {
      var v when v == typeof(CustomerV1).FullName =>
      [
        new CustomerV1
        {
          Id = id,
          FirstName = "John",
          LastName = "Doe",
          Email = "John@Doe.ca"
        }
      ],
      var v when v == typeof(CustomerV2).FullName =>
      [
        new CustomerV2
        {
          Id = id,
          FirstName = "John",
          LastName = "Doe",
          Email = "John@Doe.ca",
          Citizen = "Canada"
        }
      ],
      _ => throw new InvalidOperationException($"Type {customerType.Name} not found")
    };
  }
}

