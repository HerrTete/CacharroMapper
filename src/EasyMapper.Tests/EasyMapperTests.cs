namespace EasyMapper.Tests;

[TestClass]
public class EasyMapperTests
{
    [TestMethod]
    public void Map_SimpleObject()
    {
        var source = new { Name = "John", Age = 30 };
        var target = EasyMapper.Map<SimpleTarget>(source);
        Assert.AreEqual("John", target.Name);
        Assert.AreEqual(30, target.Age);
    }
}

public class SimpleTarget
{
    public string Name { get; set; }
    public int Age { get; set; }
}