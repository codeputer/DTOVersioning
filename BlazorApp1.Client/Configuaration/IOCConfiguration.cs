namespace BlazorApp1.Client.Configuaration;

public static class IOCConfiguration
{
  public static void CustomerIOCSetup(IServiceCollection servicesCollection)
  {
    servicesCollection.AddScoped<CustomerManager>();
    servicesCollection.AddScoped<ICustomerRA<ICustomerDTO>, CustomerRA_V1>();
    servicesCollection.AddScoped<ICustomerRA<ICustomerDTO>, CustomerRA_V2>();
    servicesCollection.AddScoped<ICustomerRA<ICustomerDTO>, CustomerRA_V3>();
    
    //servicesCollection.AddScoped<ICustomerEngine, CustomerLogicEngineV1>();
    //servicesCollection.AddScoped<ICustomerEngine, CustomerLogicEngineV2>();
    //servicesCollection.AddScoped<ICustomerEngine, CustomerLogicEngineV3>();
    servicesCollection.AddScoped<CustomerInfoEngine>();
  }
}
