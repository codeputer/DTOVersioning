using BlazorApp1.Client.Models;

namespace BlazorApp1.Client.Engines.Interfaces;

/// <summary>
/// Marker Interface to handle the management of Customer across verions
/// </summary>
//[JsonPolymorphic(TypeDiscriminatorPropertyName = "FullTypeName")]
//[JsonDerivedType(typeof(CustomerV1), "BlazorApp1.Client.Models.CustomerV1")]
//[JsonDerivedType(typeof(CustomerV2), "BlazorApp1.Client.Models.CustomerV2")]
//[JsonDerivedType(typeof(CustomerV3), "BlazorApp1.Client.Models.CustomerV3")]
public interface ICustomerDTO
{
  string Id { get; }
}
