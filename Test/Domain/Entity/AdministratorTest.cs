using MinimalApi.Domain.Entity;

namespace Test.Domain.Entity;

[TestClass]
public class AdministratorTest
{
    [TestMethod]
    public void TestGetProperties()
    {
        // Arrange
        var adm = new Administrator
        {
            // Act
            Id = 1,
            Email = "test@test.com",
            Password = "test",
            Profile = "Adm"
        };

        // Assert
        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("test@test.com", adm.Email);
        Assert.AreEqual("test", adm.Password);
        Assert.AreEqual("Adm", adm.Profile);
    }
}