using System.Text.Json;
using System.Text.Json.Serialization;

using BlazorApp1.Client.Engines.Interfaces;
using BlazorApp1.Client.Managers;

using LateralDTOSample.ResourceAccess;

using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Client.Pages;
public partial class Customers : ComponentBase
{

  [Inject]
  private CustomerManager CustomerManager { get; set; } = default!;

  private string? _customerV1;
  private string? _customerV2;
  private string? _customerV1Result;
  private string? _customerV2Result;

  private readonly JsonSerializerOptions _options = new JsonSerializerOptions
  {
    WriteIndented = true,
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
  };

  protected override async Task OnInitializedAsync()
  {
    if (RendererInfo.IsInteractive)
    {
      await Task.Delay(1000); // Simulate async work
      _customerV1 = CustomerManager.GetCustomer<CustomerV1>("1");
      _customerV2 = CustomerManager.GetCustomer<CustomerV2>("2");
    }
    base.OnInitialized();
  }

  private void OnDeserializeCustomerV1()
  {
    try
    {
      var customerV1 = JsonSerializer.Deserialize<List<CustomerV1>>(_customerV1 ?? "", _options);
      if (customerV1 != null && customerV1.Count > 0)
        _customerV1Result = $"Deserialized: {customerV1[0].FirstName} {customerV1[0].LastName} ({customerV1[0].Email})";
      else
        _customerV1Result = "Deserialization returned no data.";
    }
    catch (Exception ex)
    {
      _customerV1Result = $"Error: {ex.Message}";
    }
  }

  private void OnDeserializeCustomerV2()
  {
    try
    {
      var customerV2 = JsonSerializer.Deserialize<List<CustomerV2>>(_customerV2 ?? "", _options);
      if (customerV2 != null && customerV2.Count > 0)
        _customerV2Result = $"Deserialized: {customerV2[0].FirstName} {customerV2[0].LastName} ({customerV2[0].Email}), Citizen: {customerV2[0].Citizen}";
      else
        _customerV2Result = "Deserialization returned no data.";
    }
    catch (Exception ex)
    {
      _customerV2Result = $"Error: {ex.Message}";
    }
  }
}

