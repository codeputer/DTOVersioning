using BlazorApp1.Client.Engines;
using BlazorApp1.Client.Engines.Interfaces;
using BlazorApp1.Client.Managers;
using BlazorApp1.Client.Pages;
using BlazorApp1.Client.ResourceAccess;
using BlazorApp1.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddScoped<CustomerManager>();
builder.Services.AddScoped<ICustomerEngine, CustomerEngineV1>();
builder.Services.AddScoped<ICustomerEngine, CustomerEngineV2>();
builder.Services.AddScoped<ICustomerEngine, CustomerEngineV3>();
builder.Services.AddScoped<CustomerRA>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
     .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorApp1.Client._Imports).Assembly);

app.Run();
