using BlazorApp1.Client.Engines.Interfaces;
using BlazorApp1.Client.ResourceAccess.Interfaces;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<CustomerManager>();
builder.Services.AddScoped<ICustomerRA<ICustomerDTO>, CustomerRA_V1>();
builder.Services.AddScoped<ICustomerRA<ICustomerDTO>, CustomerRA_V2>();
builder.Services.AddScoped<ICustomerRA<ICustomerDTO>, CustomerRA_V3>();

builder.Services.AddScoped<ICustomerEngine, CustomerEngineV1>();
builder.Services.AddScoped<ICustomerEngine, CustomerEngineV2>();
builder.Services.AddScoped<ICustomerEngine, CustomerEngineV3>();
builder.Services.AddScoped<CustomerEngine>();

var blazorApp = builder.Build();



await blazorApp.RunAsync();