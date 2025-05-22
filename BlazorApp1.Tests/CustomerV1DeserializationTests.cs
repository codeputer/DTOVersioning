using System.Text.Json;

namespace BlazorApp1.Tests;

[TestClass]
public sealed class CustomerV1DeserializationTests
{
  [TestMethod]
  public void CustomerV1_DeserializeFromJson_AllPropertiesAreSet()
  {
    // Arrange: create a CustomerV1 and serialize it
    var customerV1 = new CustomerV1
    {
      Id = "abc123",
      FirstName = "Alice",
      LastName = "Smith",
      Email = "alice.smith@example.com"
    };

    var resultPayload = ResultPayloadOfType<CustomerV1>.CreateInstance(customerV1);

    var result = Result<ICustomerDTO, ResultPayloadOfType<CustomerV1>>.Success(resultPayload);
    var json = result.ResultJson;
    if (string.IsNullOrEmpty(json))
    {
      throw new InvalidOperationException("ResultJson is null or empty.");
    } 

    var deserializedResultV2 = JsonSerializer.Deserialize<ResultPayloadOfType<CustomerV1>>(json);
    var deserialized = deserializedResultV2?.Payload;

    // Assert
    Assert.IsNotNull(deserialized);
    Assert.AreEqual(customerV1.Id, deserialized!.Id);
    Assert.AreEqual(customerV1.FirstName, deserialized.FirstName);
    Assert.AreEqual(customerV1.LastName, deserialized.LastName);
    Assert.AreEqual(customerV1.Email, deserialized.Email);
  }
}
