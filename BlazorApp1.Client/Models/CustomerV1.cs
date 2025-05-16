namespace LateralDTOSample.ResourceAccess;

public class CustomerV1 : ICustomerDTO, IDTOFactoryAttributes
{
  public string Id { get; set; } = string.Empty;
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;

  public virtual string TypeName => typeof(CustomerV1).FullName ?? string.Empty;
  public virtual double Version => 1;
}



