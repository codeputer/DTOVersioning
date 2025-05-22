using BlazorApp1.Client.Models;

namespace BlazorApp1.Client.Pages;

public partial class Customers : ComponentBase
{

  [Inject]
  private CustomerManager CustomerManager { get; set; } = default!;

  private ICustomerDTO? _customerDTOV1;
  private ICustomerDTO? _customerDTOV2;
  private ICustomerDTO? _customerDTOV3;

  private string? _customerJSONV1;
  private string? _customerJSONV2;
  private string? _customerJSONV3;

  private string? _customerV1Result;
  private string? _customerV2Result;
  private string? _customerV3Result;

  Result<ICustomerDTO, ResultPayloadOfType<CustomerV1>>? _ResultPayloadV1;
  Result<ICustomerDTO, ResultPayloadOfType<CustomerV2>>? _ResultPayloadV2;
  Result<ICustomerDTO, ResultPayloadOfType<CustomerV3>>? _ResultPayloadV3;

  private readonly JsonSerializerOptions _JsonSerializeOptions = new()
  {
    WriteIndented = true,
    PropertyNameCaseInsensitive = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    //Converters = { new ResultJsonDeserializor() }
  };

  protected override async Task OnInitializedAsync()
  {
    if (RendererInfo.IsInteractive)
    {
      await Task.Delay(1000); // Simulate async work
      _customerDTOV1 = CustomerManager.GetCustomer<CustomerV1>("1");
      _customerDTOV2 = CustomerManager.GetCustomer<CustomerV2>("2");
      _customerDTOV3 = CustomerManager.GetCustomer<CustomerV3>("3");

      //wrap the ICustoemrDTO in a ResultPayloadOfType<T> object
      var _CustomerV1Wrapped = ResultPayloadOfType<CustomerV1>.CreateInstance(_customerDTOV1 as CustomerV1);
      var _CustomerV2Wrapped = ResultPayloadOfType<CustomerV2>.CreateInstance(_customerDTOV1 as CustomerV2);
      var _CustomerV3Wrapped = ResultPayloadOfType<CustomerV3>.CreateInstance(_customerDTOV1 as CustomerV3);

      Result<ICustomerDTO, ResultPayloadOfType<CustomerV1>> _ResultPayloadV1 = Result<ICustomerDTO, ResultPayloadOfType<CustomerV1>>.Success(_CustomerV1Wrapped!);
      Result<ICustomerDTO, ResultPayloadOfType<CustomerV2>> _ResultPayloadV2 = Result<ICustomerDTO, ResultPayloadOfType<CustomerV2>>.Success(_CustomerV2Wrapped!);
      Result<ICustomerDTO, ResultPayloadOfType<CustomerV3>> _ResultPayloadV3 = Result<ICustomerDTO, ResultPayloadOfType<CustomerV3>>.Success(_CustomerV3Wrapped!);

      _customerJSONV1 = _ResultPayloadV1.ResultJson;
      _customerJSONV2 = _ResultPayloadV2.ResultJson;
      _customerJSONV3 = _ResultPayloadV3.ResultJson;

    }
    base.OnInitialized();
  }

  private void OnDeserializeCustomerV1()
  {
    try
    {
      if (string.IsNullOrWhiteSpace(_customerV1Result) == false)
      {
        _customerV1Result = "Deserialization returned null.";
        return;
      }

      if (_ResultPayloadV1!.IsSuccessful)
      {
        var customerV1 = _ResultPayloadV1!.PayloadWrapper!.Payload;
        _customerV1Result = $"Deserialized: {customerV1.FirstName} {customerV1.LastName} ({customerV1.Email})";
      }
      else
      {
        _customerV1Result = $"Deserialization failed: {string.Join(", ", _ResultPayloadV1.Messages)}";
      }
    }
    catch (Exception ex)
    {
      _customerV1Result = $"Error: {ex.Message}";
    }
  }

  private void OnDeserializeCustomerV2()
  {

    CustomerEngineV2? customerV2Result = null;
    //try
    //{
    //  var customerV1Result = JsonSerializer.Deserialize<Result<List<CustomerV2>>>(_customerV2 ?? "", _JsonSerializeOptions);
    //  if (customerV2Result is null)
    //  {
    //    _customerV2Result = "Deserialization returned null.";
    //    return;
    //  }

    //  var listOfCustomerV2 = customerV2Result.ResultValue;
    //  if (customerV2Result.IsSuccessful)
    //  {
    //    if (listOfCustomerV2!.Count > 0)
    //      _customerV2Result = $"Deserialized: {listOfCustomerV2[0].FirstName} {listOfCustomerV2[0].LastName} ({listOfCustomerV2[0].Email})";
    //    else
    //      _customerV2Result = "Deserialization returned no data.";
    //  }
    //  else
    //  {
    //    _customerV2Result = $"Deserialization failed: {string.Join(", ", customerV2Result.Messages)}";
    //  }
    //}
    //catch (Exception ex)
    //{
    //  _customerV2Result = $"Error: {ex.Message}";
    //}
  }
  private void OnDeserializeCustomerV3()
  {
    CustomerV3 customerV3 = null;
    //try
    //{
    //  var customerV3Result = JsonSerializer.Deserialize<Result<List<CustomerV3>>>(_customerV3 ?? "", _JsonSerializeOptions);
    //  if (customerV3Result is null)
    //  {
    //    _customerV3Result = "Deserialization returned null.";
    //    return;
    //  }

    //  var listOfCustomerV3 = customerV3Result.ResultValue;
    //  if (customerV3Result.IsSuccessful)
    //  {
    //    if (listOfCustomerV3!.Count > 0)
    //      _customerV3Result = $"Deserialized: {listOfCustomerV3[0].FirstName} {listOfCustomerV3[0].LastName} ({listOfCustomerV3[0].Email})";
    //    else
    //      _customerV3Result = "Deserialization returned no data.";
    //  }
    //  else
    //  {
    //    _customerV3Result = $"Deserialization failed: {string.Join(", ", customerV3Result.Messages)}";
    //  }
    //}
    //catch (Exception ex)
    //{
    //  _customerV3Result = $"Error: {ex.Message}";
    //}
  }
}

