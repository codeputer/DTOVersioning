using BlazorApp1.Client.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorApp1.Client.Utilities;

namespace BlazorApp1.Client.Pages;

public partial class Customers : ComponentBase
{

  [Inject]
  private CustomerManager CustomerManager { get; set; } = default!;

  private string? _customerV1;
  private string? _customerV2;
  private string? _customerV3;
  private string? _customerV1Result;
  private string? _customerV2Result;
  private string? _customerV3Result;

  private readonly JsonSerializerOptions _JsonSerializeOptions = new()
  {
    WriteIndented = true,
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    Converters = { new ResultJsonDeserializor() }
  };

  protected override async Task OnInitializedAsync()
  {
    if (RendererInfo.IsInteractive)
    {
      await Task.Delay(1000); // Simulate async work
      _customerV1 = CustomerManager.GetCustomer<CustomerV1>("1");
      _customerV2 = CustomerManager.GetCustomer<CustomerV2>("2");
      _customerV3 = CustomerManager.GetCustomer<CustomerV3>("3");
    }
    base.OnInitialized();
  }
  
  private void OnDeserializeCustomerV1()
  {
    try
    {
      var customerV1Result = ResultExtensions.DeserializeResult<List<CustomerV1>>(_customerV1!, typeof(IEnumerable<CustomerV2>));
      if (customerV1Result is null)
      {
        _customerV1Result = "Deserialization returned null.";
        return;
      }

      var listOfCustomerV1 = customerV1Result.ResultValue;
      if (customerV1Result.IsSuccessful)
      {
        if (listOfCustomerV1!.Count > 0)
          _customerV1Result = $"Deserialized: {listOfCustomerV1[0].FirstName} {listOfCustomerV1[0].LastName} ({listOfCustomerV1[0].Email})";
        else
          _customerV1Result = "Deserialization returned no data.";
      }
      else
      {
        _customerV1Result = $"Deserialization failed: {string.Join(", ", customerV1Result.Messages)}";
      }
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
      var customerV2Result = JsonSerializer.Deserialize<Result<List<CustomerV2>>>(_customerV2 ?? "", _JsonSerializeOptions);
      if (customerV2Result is null)
      {
        _customerV2Result = "Deserialization returned null.";
        return;
      }

      var listOfCustomerV2 = customerV2Result.ResultValue;
      if (customerV2Result.IsSuccessful)
      {
        if (listOfCustomerV2!.Count > 0)
          _customerV2Result = $"Deserialized: {listOfCustomerV2[0].FirstName} {listOfCustomerV2[0].LastName} ({listOfCustomerV2[0].Email})";
        else
          _customerV2Result = "Deserialization returned no data.";
      }
      else
      {
        _customerV2Result = $"Deserialization failed: {string.Join(", ", customerV2Result.Messages)}";
      }
    }
    catch (Exception ex)
    {
      _customerV2Result = $"Error: {ex.Message}";
    }
  }
  private void OnDeserializeCustomerV3()
  {
    try
    {
      var customerV3Result = JsonSerializer.Deserialize<Result<List<CustomerV3>>>(_customerV3 ?? "", _JsonSerializeOptions);
      if (customerV3Result is null)
      {
        _customerV3Result = "Deserialization returned null.";
        return;
      }

      var listOfCustomerV3 = customerV3Result.ResultValue;
      if (customerV3Result.IsSuccessful)
      {
        if (listOfCustomerV3!.Count > 0)
          _customerV3Result = $"Deserialized: {listOfCustomerV3[0].FirstName} {listOfCustomerV3[0].LastName} ({listOfCustomerV3[0].Email})";
        else
          _customerV3Result = "Deserialization returned no data.";
      }
      else
      {
        _customerV3Result = $"Deserialization failed: {string.Join(", ", customerV3Result.Messages)}";
      }
    }
    catch (Exception ex)
    {
      _customerV3Result = $"Error: {ex.Message}";
    }
  }
}

