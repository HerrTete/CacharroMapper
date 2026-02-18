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
        Assert.AreEqual("Jane", target.Child.Name);
        Assert.AreEqual(5, target.Child.Age);
    }
    [TestMethod]
    public void Map_SimpleObjectWithList()
    {
        var source = new SimpleSource { Name = "John", Age = 30, Child = new SimpleSource { Name = "Jane", Age = 5 }, Children = new List<SimpleSource> { new SimpleSource { Name = "Jack", Age = 3 } } };
        var target = EasyMapper.Map<SimpleTarget>(source);
        Assert.AreEqual("John", target.Name);
        Assert.AreEqual(30, target.Age);
        Assert.AreEqual("Jane", target.Child.Name);
        Assert.AreEqual(5, target.Child.Age);
        Assert.AreEqual("Jack", target.Children[0].Name);
        Assert.AreEqual(3, target.Children[0].Age);
    }

    [TestMethod]
    public void Map_ObjectWithArray()
    {
        var source = new SourceWithArray 
        { 
            Name = "John", 
            Numbers = new[] { 1, 2, 3, 4, 5 },
            Items = new SimpleSource[] 
            { 
                new SimpleSource { Name = "Item1", Age = 10 },
                new SimpleSource { Name = "Item2", Age = 20 }
            }
        };
        var target = EasyMapper.Map<TargetWithArray>(source);
        Assert.AreEqual("John", target.Name);
        Assert.IsNotNull(target.Numbers);
        Assert.AreEqual(5, target.Numbers.Length);
        Assert.AreEqual(1, target.Numbers[0]);
        Assert.AreEqual(5, target.Numbers[4]);
        Assert.IsNotNull(target.Items);
        Assert.AreEqual(2, target.Items.Length);
        Assert.AreEqual("Item1", target.Items[0].Name);
        Assert.AreEqual(10, target.Items[0].Age);
        Assert.AreEqual("Item2", target.Items[1].Name);
        Assert.AreEqual(20, target.Items[1].Age);
    }

    [TestMethod]
    public void Map_ObjectWithDictionary()
    {
        var source = new SourceWithDictionary
        {
            Name = "John",
            Properties = new Dictionary<string, string>
            {
                { "Key1", "Value1" },
                { "Key2", "Value2" }
            },
            ComplexDictionary = new Dictionary<string, SimpleSource>
            {
                { "First", new SimpleSource { Name = "FirstItem", Age = 15 } },
                { "Second", new SimpleSource { Name = "SecondItem", Age = 25 } }
            }
        };
        var target = EasyMapper.Map<TargetWithDictionary>(source);
        Assert.AreEqual("John", target.Name);
        Assert.IsNotNull(target.Properties);
        Assert.AreEqual(2, target.Properties.Count);
        Assert.AreEqual("Value1", target.Properties["Key1"]);
        Assert.AreEqual("Value2", target.Properties["Key2"]);
        Assert.IsNotNull(target.ComplexDictionary);
        Assert.AreEqual(2, target.ComplexDictionary.Count);
        Assert.AreEqual("FirstItem", target.ComplexDictionary["First"].Name);
        Assert.AreEqual(15, target.ComplexDictionary["First"].Age);
        Assert.AreEqual("SecondItem", target.ComplexDictionary["Second"].Name);
        Assert.AreEqual(25, target.ComplexDictionary["Second"].Age);
    }

    [TestMethod]
    public void Map_ObjectWithIList()
    {
        var source = new SourceWithIList
        {
            Name = "John",
            Items = new List<SimpleSource>
            {
                new SimpleSource { Name = "IListItem1", Age = 30 },
                new SimpleSource { Name = "IListItem2", Age = 40 }
            }
        };
        var target = EasyMapper.Map<TargetWithIList>(source);
        Assert.AreEqual("John", target.Name);
        Assert.IsNotNull(target.Items);
        Assert.AreEqual(2, target.Items.Count);
        Assert.AreEqual("IListItem1", target.Items[0].Name);
        Assert.AreEqual(30, target.Items[0].Age);
        Assert.AreEqual("IListItem2", target.Items[1].Name);
        Assert.AreEqual(40, target.Items[1].Age);
    }

    [TestMethod]
    public void Map_ObjectWithEmptyArray()
    {
        var source = new SourceWithArray
        {
            Name = "Empty",
            Numbers = new int[0],
            Items = new SimpleSource[0]
        };
        var target = EasyMapper.Map<TargetWithArray>(source);
        Assert.AreEqual("Empty", target.Name);
        Assert.IsNotNull(target.Numbers);
        Assert.AreEqual(0, target.Numbers.Length);
        Assert.IsNotNull(target.Items);
        Assert.AreEqual(0, target.Items.Length);
    }

    [TestMethod]
    public void Map_ObjectWithEmptyDictionary()
    {
        var source = new SourceWithDictionary
        {
            Name = "Empty",
            Properties = new Dictionary<string, string>(),
            ComplexDictionary = new Dictionary<string, SimpleSource>()
        };
        var target = EasyMapper.Map<TargetWithDictionary>(source);
        Assert.AreEqual("Empty", target.Name);
        Assert.IsNotNull(target.Properties);
        Assert.AreEqual(0, target.Properties.Count);
        Assert.IsNotNull(target.ComplexDictionary);
        Assert.AreEqual(0, target.ComplexDictionary.Count);
    }

    [TestMethod]
    public void Map_ObjectWithEmptyIList()
    {
        var source = new SourceWithIList
        {
            Name = "Empty",
            Items = new List<SimpleSource>()
        };
        var target = EasyMapper.Map<TargetWithIList>(source);
        Assert.AreEqual("Empty", target.Name);
        Assert.IsNotNull(target.Items);
        Assert.AreEqual(0, target.Items.Count);
    }
}

[TestClass]
public class EasyMapperWithNameMappingTests
{
    [TestMethod]
    public void Map_ObjectWithDifferentPropertyNames()
    {
        var source = new { FirstName = "John", Years = 30 };
        var target = EasyMapper.Map<SimpleTarget>(source, new List<PropertyNameMapping>
        {
            new PropertyNameMapping { SourcePropertyName = "FirstName", TargetPropertyName = "Name" },
            new PropertyNameMapping { SourcePropertyName = "Years", TargetPropertyName = "Age" }
         });
        Assert.AreEqual("John", target.Name); // Assuming we have a mapping for FirstName -> Name
        Assert.AreEqual(30, target.Age); // Assuming we have a mapping for Years -> Age
    }
}

public class SimpleSource
{
    public string Name { get; set; }
    public int Age { get; set; }

    public SimpleSource Child { get; set; }

    public List<SimpleSource> Children { get; set; }
}

public class SimpleTarget
{
    public string Name { get; set; }
    public int Age { get; set; }
    public SimpleTarget Child { get; set; }
    public List<SimpleTarget> Children { get; set; }
}

public class SourceWithArray
{
    public string Name { get; set; }
    public int[] Numbers { get; set; }
    public SimpleSource[] Items { get; set; }
}

public class TargetWithArray
{
    public string Name { get; set; }
    public int[] Numbers { get; set; }
    public SimpleTarget[] Items { get; set; }
}

public class SourceWithDictionary
{
    public string Name { get; set; }
    public Dictionary<string, string> Properties { get; set; }
    public Dictionary<string, SimpleSource> ComplexDictionary { get; set; }
}

public class TargetWithDictionary
{
    public string Name { get; set; }
    public Dictionary<string, string> Properties { get; set; }
    public Dictionary<string, SimpleTarget> ComplexDictionary { get; set; }
}

public class SourceWithIList
{
    public string Name { get; set; }
    public IList<SimpleSource> Items { get; set; }
}

public class TargetWithIList
{
    public string Name { get; set; }
    public IList<SimpleTarget> Items { get; set; }
}