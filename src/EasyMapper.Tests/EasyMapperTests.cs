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
    
    [TestMethod]
    public void Map_SimpleObjectWithChild()
    {
        var source = new SimpleSource { Name = "John", Age = 30, Child = new SimpleSource { Name = "Jane", Age = 5 } };
        var target = EasyMapper.Map<SimpleTarget>(source);
        Assert.AreEqual("John", target.Name);
        Assert.AreEqual(30, target.Age);
        Assert.AreEqual("Jane", target.Child!.Name);
        Assert.AreEqual(5, target.Child!.Age);
    }
    [TestMethod]
    public void Map_SimpleObjectWithList()
    {
        var source = new SimpleSource { Name = "John", Age = 30, Child = new SimpleSource { Name = "Jane", Age = 5 }, Children = new List<SimpleSource> { new SimpleSource { Name = "Jack", Age = 3 } } };
        var target = EasyMapper.Map<SimpleTarget>(source);
        Assert.AreEqual("John", target.Name);
        Assert.AreEqual(30, target.Age);
        Assert.AreEqual("Jane", target.Child!.Name);
        Assert.AreEqual(5, target.Child!.Age);
        Assert.AreEqual("Jack", target.Children![0].Name);
        Assert.AreEqual(3, target.Children![0].Age);
    }
}

public class SimpleSource
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }

    public SimpleSource? Child { get; set; }

    public List<SimpleSource>? Children { get; set; }
}

public class SimpleTarget
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public SimpleTarget? Child { get; set; }
    public List<SimpleTarget>? Children { get; set; }
}