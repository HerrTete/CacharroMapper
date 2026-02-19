using CacharroMapper;

namespace CacharroMapper.Tests;

[TestClass]
public class CacharroMapperExtensionTests
{
    [TestMethod]
    public void Map_SimpleObjectWithExtensionMethod()
    {
        var source = new { Name = "John", Age = 30 };
        var target = source.Map<SimpleTarget>();
        Assert.AreEqual("John", target.Name);
        Assert.AreEqual(30, target.Age);
    }
}