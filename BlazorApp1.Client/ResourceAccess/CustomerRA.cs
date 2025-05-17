using BlazorApp1.Client.Utilities;

namespace BlazorApp1.Client.ResourceAccess;

public class CustomerRA
{
  public Result<IEnumerable<ICustomerDTO>> GetCustomer<T>(string id) where T : class, ICustomerDTO, new()
  {
    //capture the type required for use in switch statement and messages
    var typeRequired = typeof(T);

    return typeRequired switch
    {
      var _ when typeRequired == typeof(CustomerV1) => Result<IEnumerable<ICustomerDTO>>.Success(
                                                                [
                                                                  new CustomerV2
                                                                  {
                                                                    Id = id,
                                                                    FirstName = $"John_{id}",
                                                                    LastName = "Doe",
                                                                    Email = $"John_{id}@Doe.ca",
                                                                    Citizen = "Canada"
                                                                  }
                                                                ]),

      var _ when typeRequired == typeof(CustomerV2) => Result<IEnumerable<ICustomerDTO>>.Success(
                                                                [
                                                                  new CustomerV2
                                                                  {
                                                                    Id = id,
                                                                    FirstName = $"John_{id}",
                                                                    LastName = "Doe",
                                                                    Email = $"John_{id}@Doe.ca",
                                                                    Citizen = "Canada"
                                                                  }
                                                                ]),

      var _ when typeRequired == typeof(CustomerV3) => Result<IEnumerable<ICustomerDTO>>.Failure($"Customer Id:{id.ParamMarkers()} was not found"),

      _ => throw new NotImplementedException($"The requested type:{typeRequired} is not implemented in the CustomerRA class"),
    };
  }
}


