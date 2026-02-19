namespace CacharroMapper.Tests;

[TestClass]
public class CacharroMapperTests
{
    [TestMethod]
    public void MapList_ListOfObjects()
    {
        var sources = new List<SimpleSource>
        {
            new SimpleSource { Name = "John", Age = 30 },
            new SimpleSource { Name = "Jane", Age = 25 }
        };
        var targets = CacharroMapper.MapList<SimpleTarget>(sources);
        Assert.AreEqual(2, targets.Count);
        Assert.AreEqual("John", targets[0].Name);
        Assert.AreEqual(30, targets[0].Age);
        Assert.AreEqual("Jane", targets[1].Name);
        Assert.AreEqual(25, targets[1].Age);
    }

    [TestMethod]
    public void Map_SimpleObject()
    {
        var source = new { Name = "John", Age = 30 };
        var target = CacharroMapper.Map<SimpleTarget>(source);
        Assert.AreEqual("John", target.Name);
        Assert.AreEqual(30, target.Age);
    }
    
    [TestMethod]
    public void Map_SimpleObjectWithChild()
    {
        var source = new SimpleSource { Name = "John", Age = 30, Child = new SimpleSource { Name = "Jane", Age = 5 } };
        var target = CacharroMapper.Map<SimpleTarget>(source);
        Assert.AreEqual("John", target.Name);
        Assert.AreEqual(30, target.Age);
        Assert.IsNotNull(target.Child);
        Assert.AreEqual("Jane", target.Child.Name);
        Assert.AreEqual(5, target.Child.Age);
    }
    [TestMethod]
    public void Map_SimpleObjectWithList()
    {
        var source = new SimpleSource { Name = "John", Age = 30, Child = new SimpleSource { Name = "Jane", Age = 5 }, Children = new List<SimpleSource> { new SimpleSource { Name = "Jack", Age = 3 } } };
        var target = CacharroMapper.Map<SimpleTarget>(source);
        Assert.AreEqual("John", target.Name);
        Assert.AreEqual(30, target.Age);
        Assert.IsNotNull(target.Child);
        Assert.AreEqual("Jane", target.Child.Name);
        Assert.AreEqual(5, target.Child.Age);
        Assert.IsNotNull(target.Children);
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
        var target = CacharroMapper.Map<TargetWithArray>(source);
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
        var target = CacharroMapper.Map<TargetWithDictionary>(source);
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
        var target = CacharroMapper.Map<TargetWithIList>(source);
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
        var target = CacharroMapper.Map<TargetWithArray>(source);
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
        var target = CacharroMapper.Map<TargetWithDictionary>(source);
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
        var target = CacharroMapper.Map<TargetWithIList>(source);
        Assert.AreEqual("Empty", target.Name);
        Assert.IsNotNull(target.Items);
        Assert.AreEqual(0, target.Items.Count);
    }
}

[TestClass]
public class CacharroMapperWithNameMappingTests
{
    [TestMethod]
    public void Map_ObjectWithDifferentPropertyNames()
    {
        var source = new { FirstName = "John", Years = 30 };
        var target = CacharroMapper.Map<SimpleTarget>(source, new List<PropertyNameMapping>
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
    public string? Name { get; set; }
    public int Age { get; set; }

    public SimpleSource? Child { get; set; }

    public List<SimpleSource>? Children { get; set; }
}

public class SimpleTarget
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public SimpleTarget? Child { get; set; }
    public List<SimpleTarget>? Children { get; set; }
}

public class SourceWithArray
{
    public string? Name { get; set; }
    public int[]? Numbers { get; set; }
    public SimpleSource[]? Items { get; set; }
}

public class TargetWithArray
{
    public string? Name { get; set; }
    public int[]? Numbers { get; set; }
    public SimpleTarget[]? Items { get; set; }
}

public class SourceWithDictionary
{
    public string? Name { get; set; }
    public Dictionary<string, string>? Properties { get; set; }
    public Dictionary<string, SimpleSource>? ComplexDictionary { get; set; }
}

public class TargetWithDictionary
{
    public string? Name { get; set; }
    public Dictionary<string, string>? Properties { get; set; }
    public Dictionary<string, SimpleTarget>? ComplexDictionary { get; set; }
}

public class SourceWithIList
{
    public string? Name { get; set; }
    public IList<SimpleSource>? Items { get; set; }
}

public class TargetWithIList
{
    public string? Name { get; set; }
    public IList<SimpleTarget>? Items { get; set; }
}

[TestClass]
public class CacharroMapperEnumTests
{
    [TestMethod]
    public void Map_EnumByName_SameEnumType()
    {
        var source = new SourceWithEnum { Name = "Test", Status = SourceStatus.Active };
        var target = CacharroMapper.Map<TargetWithEnum>(source);
        Assert.AreEqual("Test", target.Name);
        Assert.AreEqual(TargetStatus.Active, target.Status);
    }

    [TestMethod]
    public void Map_EnumByName_DifferentEnumTypes()
    {
        var source = new SourceWithEnum { Name = "Test", Status = SourceStatus.Inactive };
        var target = CacharroMapper.Map<TargetWithEnum>(source);
        Assert.AreEqual("Test", target.Name);
        Assert.AreEqual(TargetStatus.Inactive, target.Status);
    }

    [TestMethod]
    public void Map_EnumByName_WithDifferentIntValues()
    {
        // SourceStatus.Pending has value 2, TargetStatus.Pending has value 10
        var source = new SourceWithEnum { Name = "Test", Status = SourceStatus.Pending };
        var target = CacharroMapper.Map<TargetWithEnum>(source);
        Assert.AreEqual("Test", target.Name);
        Assert.AreEqual(TargetStatus.Pending, target.Status);
        // Verify that we're mapping by name, not by int value
        Assert.AreEqual((int)TargetStatus.Pending, 10);
        Assert.AreNotEqual((int)SourceStatus.Pending, (int)TargetStatus.Pending);
    }

    [TestMethod]
    public void Map_EnumByName_NonExistentValue()
    {
        var source = new SourceWithEnum { Name = "Test", Status = SourceStatus.Archived };
        var target = CacharroMapper.Map<TargetWithEnum>(source);
        Assert.AreEqual("Test", target.Name);
        // Should map to default value (first enum value) when name doesn't exist
        Assert.AreEqual(default(TargetStatus), target.Status);
    }

    [TestMethod]
    public void Map_ObjectWithEnumArray()
    {
        var source = new SourceWithEnumArray
        {
            Name = "Test",
            Statuses = new[] { SourceStatus.Active, SourceStatus.Pending, SourceStatus.Inactive }
        };
        var target = CacharroMapper.Map<TargetWithEnumArray>(source);
        Assert.AreEqual("Test", target.Name);
        Assert.IsNotNull(target.Statuses);
        Assert.AreEqual(3, target.Statuses.Length);
        Assert.AreEqual(TargetStatus.Active, target.Statuses[0]);
        Assert.AreEqual(TargetStatus.Pending, target.Statuses[1]);
        Assert.AreEqual(TargetStatus.Inactive, target.Statuses[2]);
    }

    [TestMethod]
    public void Map_ObjectWithEnumList()
    {
        var source = new SourceWithEnumList
        {
            Name = "Test",
            Statuses = new List<SourceStatus> { SourceStatus.Active, SourceStatus.Inactive }
        };
        var target = CacharroMapper.Map<TargetWithEnumList>(source);
        Assert.AreEqual("Test", target.Name);
        Assert.IsNotNull(target.Statuses);
        Assert.AreEqual(2, target.Statuses.Count);
        Assert.AreEqual(TargetStatus.Active, target.Statuses[0]);
        Assert.AreEqual(TargetStatus.Inactive, target.Statuses[1]);
    }
}

public enum SourceStatus
{
    Active = 0,
    Inactive = 1,
    Pending = 2,
    Archived = 3
}

public enum TargetStatus
{
    Active = 0,
    Inactive = 1,
    Pending = 10  // Different int value than SourceStatus.Pending
}

public class SourceWithEnum
{
    public string? Name { get; set; }
    public SourceStatus Status { get; set; }
}

public class TargetWithEnum
{
    public string? Name { get; set; }
    public TargetStatus Status { get; set; }
}

public class SourceWithEnumArray
{
    public string? Name { get; set; }
    public SourceStatus[]? Statuses { get; set; }
}

public class TargetWithEnumArray
{
    public string? Name { get; set; }
    public TargetStatus[]? Statuses { get; set; }
}

public class SourceWithEnumList
{
    public string? Name { get; set; }
    public List<SourceStatus>? Statuses { get; set; }
}

public class TargetWithEnumList
{
    public string? Name { get; set; }
    public List<TargetStatus>? Statuses { get; set; }
}