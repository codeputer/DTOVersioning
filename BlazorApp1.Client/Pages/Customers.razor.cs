namespace BlazorApp1.Client.Pages;

public partial class Customers : ComponentBase
{

  [Inject]
  private CustomerManager CustomerManager { get; set; } = default!;

  private string? _customerJSONV1;
  private string? _customerJSONV2;
  private string? _customerJSONV3;

  private string? _customerV1Deserialized;
  private string? _customerV2Deserialized;
  private string? _customerV3Deserialized;

  Result<ICustomerDTO, ResultPayloadOfType<ICustomerDTO>> resultV1 = default!;
  Result<ICustomerDTO, ResultPayloadOfType<ICustomerDTO>> resultV2 = default!;
  Result<ICustomerDTO, ResultPayloadOfType<ICustomerDTO>> resultV3 = default!;


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
      resultV1 = CustomerManager.GetCustomer<CustomerV1>("1");
      resultV2 = CustomerManager.GetCustomer<CustomerV2>("2");
      resultV3 = CustomerManager.GetCustomer<CustomerV3>("3");

      resultV1.Serialize();
      resultV2.Serialize();
      resultV3.Serialize();

      _customerJSONV1 = resultV1.ResultJson;
      _customerJSONV2 = resultV2.ResultJson;
      _customerJSONV3 = resultV3.ResultJson;

    }
    base.OnInitialized();
  }

  private void OnDeserializeCustomerV1()
  {
    try
    {
      if (string.IsNullOrWhiteSpace(_customerJSONV1))
      {
        _customerJSONV1 = "Deserialization returned null.";
        return;
      }

      if (resultV1!.IsSuccessful)
      {
        Type fulltype = Type.GetType(resultV1.FullTypeName!)
                              ?? throw new InvalidOperationException($"Type {resultV1.FullTypeName} not found.");

        var customerV1 = JsonSerializer.Deserialize(_customerJSONV1, fulltype) as CustomerV1;

        if (customerV1 is not null)
        {
          _customerV1Deserialized = $"Deserialized: {customerV1.FirstName} {customerV1.LastName} ({customerV1.Email})";
        }
        else
        {
          _customerV1Deserialized = "Deserialization returned an unexpected type.";
        }
      }

    }
    catch (Exception ex)
    {
      _customerV1Deserialized = $"Error: {ex.Message}";
    }
  }
  private void OnDeserializeCustomerV2()
  {
    try
    {
      if (string.IsNullOrWhiteSpace(_customerJSONV2))
      {
        _customerJSONV2 = "Deserialization returned null.";
        return;
      }

      if (resultV2!.IsSuccessful)
      {
        Type fulltype = Type.GetType(resultV1.FullTypeName!)
                              ?? throw new InvalidOperationException($"Type {resultV1.FullTypeName} not found.");

        var customerV2 = JsonSerializer.Deserialize(_customerJSONV2, fulltype) as CustomerV2;

        if (customerV2 is not null)
        {
          _customerV2Deserialized = $"Deserialized: {customerV2.FirstName} {customerV2.LastName} ({customerV2.Email} Citizen:{customerV2.Citizen})";
        }
        else
        {
          _customerV2Deserialized = "Deserialization returned an unexpected type.";
        }
      }

    }
    catch (Exception ex)
    {
      _customerV1Deserialized = $"Error: {ex.Message}";
    }
  }

  private void OnDeserializeCustomerV3()
  {
    try
    {
      if (string.IsNullOrWhiteSpace(_customerJSONV3))
      {
        _customerJSONV3 = "Deserialization returned null.";
        return;
      }

      if (resultV3!.IsSuccessful)
      {
        Type fulltype = Type.GetType(resultV3.FullTypeName!)
                              ?? throw new InvalidOperationException($"Type {resultV1.FullTypeName} not found.");

        var customerV3 = JsonSerializer.Deserialize(_customerJSONV3, fulltype) as CustomerV3;

        if (customerV3 is not null)
        {
          _customerV3Deserialized = $"Deserialized: {customerV3.FirstName} {customerV3.LastName} ({customerV3.Email} Heritage:{customerV3.Heritage})";
        }
        else
        {
          _customerV3Deserialized = "Deserialization returned an unexpected type.";
        }
      }
      else
      {
        _customerV3Deserialized = "Deserialization failed: " + string.Join(", ", resultV3.Messages);
      }

    }
    catch (Exception ex)
    {
      _customerV3Deserialized = $"Error: {ex.Message}";
    }
  }

}

