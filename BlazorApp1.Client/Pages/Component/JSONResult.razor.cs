namespace BlazorApp1.Client.Pages.Component;
public partial class JSONResult
{

  [Parameter] public string Label { get; set; } = string.Empty;
  [Parameter] public string ButtonLabel { get; set; } = "Deserialize";
  [Parameter] public string? CustomerJson { get; set; }
  [Parameter] public string? DeserializationResult { get; set; }
  [Parameter] public EventCallback OnButtonClick { get; set; }
  [Parameter] public string TextAreaId { get; set; } = Guid.NewGuid().ToString();
}

