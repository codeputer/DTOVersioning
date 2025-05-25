using BlazorApp1.Client.Configuaration;
using BlazorApp1.Client.Engines.Interfaces;
using BlazorApp1.Client.ResourceAccess.Interfaces;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

IOCConfiguration.CustomerIOCSetup(builder.Services);

var blazorApp = builder.Build();

await blazorApp.RunAsync();