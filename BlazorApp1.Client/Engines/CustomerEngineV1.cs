using System.Collections.Generic;

using BlazorApp1.Client.Models;
using BlazorApp1.Client.Utilities;

namespace BlazorApp1.Client.Engines;

public class CustomerEngineV1(CustomerRA customerRA) : ICustomerEngine
{
  private readonly CustomerRA _CustomerRA = customerRA ?? throw new ArgumentNullException(nameof(customerRA));

  public Type CustomerType => typeof(CustomerV1);

  public Result<IEnumerable<ICustomerDTO>> GetCustomer(string id)
  {
     return  _CustomerRA.GetCustomer<CustomerV1>(id);
  }
}


