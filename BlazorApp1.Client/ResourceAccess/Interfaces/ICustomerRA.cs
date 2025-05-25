namespace BlazorApp1.Client.ResourceAccess.Interfaces;

public interface ICustomerRA<TReturnDTOVersion> where TReturnDTOVersion : ICustomerDTO
{
  string CustomerVersionDTOType { get;  }

  /// <summary>
  /// Gets the customer of type T.
  /// </summary>
  /// <param name="id">The customer id.</param>
  /// <returns>The customer of type T.</returns>
  public ICustomerDTO GetCustomer(string id);

  /// <summary>
  /// Gets the customers of type T.
  /// </summary>
  /// <returns>The list of customers of type T.</returns>
   IEnumerable<ICustomerDTO> GetCustomers();
}

