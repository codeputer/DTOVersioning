using BlazorApp1.Client.Engines;
using BlazorApp1.Client.Engines.Interfaces;
using BlazorApp1.Client.Managers;
using BlazorApp1.Client.ResourceAccess;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<CustomerManager>();
builder.Services.AddScoped<ICustomerEngine, CustomerEngineV1>();
builder.Services.AddScoped<ICustomerEngine, CustomerEngineV2>();
builder.Services.AddScoped<CustomerRA>();

var blazorApp = builder.Build();



await blazorApp.RunAsync();