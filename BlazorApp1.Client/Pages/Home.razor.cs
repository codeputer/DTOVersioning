
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Client.Pages;
public partial class Home : ComponentBase
{
  public readonly ILogger<Home> Logger;
  private readonly List<string> transitionLog = [];
  private string lastPhase = "";

  public Home(ILogger<Home> logger)
  {
    Logger = logger;
    lastPhase = "PreRender";
    transitionLog.Add($"Phase:{lastPhase} Interactive:false");
  }

  protected override async Task OnAfterRenderAsync(bool firstRender)
  {
    string phase;
    if (RendererInfo is null)
      return;

    phase = RendererInfoName;
    
    if (phase != lastPhase)
    {
      string logMsg = $"Phase:{phase} Interactive:{RendererInfoIsInteractive}";
      Logger.LogInformation(logMsg);
      transitionLog.Add(logMsg);
      lastPhase = phase;
      await Task.Delay(2000); // Simulate some async work
      StateHasChanged(); // Trigger a re-render to update the UI, as we just finished a render
    }
  }

  // Expose RendererInfo for the UI
  public string RendererInfoName => RendererInfo?.Name ?? "null";
  public bool RendererInfoIsInteractive => RendererInfo?.IsInteractive ?? false;
  public IReadOnlyList<string> TransitionLog => transitionLog;
}
