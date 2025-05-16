namespace LateralDTOSample.ResourceAccess;

public class CustomerV2 : CustomerV1, IDTOFactoryAttributes
{ 
  public string Citizen { get; set; } = string.Empty;

  public override string TypeName => typeof(CustomerV2).FullName ?? string.Empty;

  public override double Version => 2;
}



