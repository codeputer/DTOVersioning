namespace BlazorApp1.Client.Engines.Interfaces;

/// <summary>
/// Marker Interface to handle the management of Customer across verions
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "typeName")]
[JsonDerivedType(typeof(CustomerV1), "LateralDTOSample.ResourceAccess.CustomerV1")]
[JsonDerivedType(typeof(CustomerV2), "LateralDTOSample.ResourceAccess.CustomerV2")]
public interface ICustomerDTO;
