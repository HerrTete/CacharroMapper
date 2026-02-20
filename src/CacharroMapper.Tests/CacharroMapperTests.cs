using System.Data.Common;
using System.Linq.Expressions;

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

    public string Hans { get; set; }

    public SourceWithEnumList List { get; set; }
}

public class TargetWithEnumArray
{
    public string? Name { get; set; }
    public TargetStatus[]? Statuses { get; set; }

    public string Hans { get; set; }

    public TargetWithEnumList List { get; set; }
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

public class SourceItemWithTags
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public string[]? Tags { get; set; }
}

public class TargetItemWithTags
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public string[]? Tags { get; set; }
}

public class SourceWithComplexItemList
{
    public string? Name { get; set; }
    public List<SourceItemWithTags>? Items { get; set; }
}

public class TargetWithComplexItemList
{
    public string? Name { get; set; }
    public List<TargetItemWithTags>? Items { get; set; }
}

public class SourceWithDictionaryOfLists
{
    public string? Name { get; set; }
    public Dictionary<string, List<SimpleSource>>? Groups { get; set; }
}

public class TargetWithDictionaryOfLists
{
    public string? Name { get; set; }
    public Dictionary<string, List<SimpleTarget>>? Groups { get; set; }
}

public class SourceWithDictionaryOfArrays
{
    public string? Name { get; set; }
    public Dictionary<string, SimpleSource[]>? Groups { get; set; }
}

public class TargetWithDictionaryOfArrays
{
    public string? Name { get; set; }
    public Dictionary<string, SimpleTarget[]>? Groups { get; set; }
}

public class SourceWithListOfDictionaryOwners
{
    public string? Name { get; set; }
    public List<SourceWithDictionary>? Items { get; set; }
}

public class TargetWithListOfDictionaryOwners
{
    public string? Name { get; set; }
    public List<TargetWithDictionary>? Items { get; set; }
}

[TestClass]
public class CacharroMapperCollectionCombinationTests
{
    [TestMethod]
    public void MapList_WithComplexObjectsThatHaveChildren()
    {
        var sources = new List<SimpleSource>
        {
            new SimpleSource
            {
                Name = "Parent1", Age = 30,
                Child = new SimpleSource { Name = "Child1", Age = 5 },
                Children = new List<SimpleSource> { new SimpleSource { Name = "SubChild1", Age = 1 } }
            },
            new SimpleSource
            {
                Name = "Parent2", Age = 25,
                Child = new SimpleSource { Name = "Child2", Age = 3 },
                Children = new List<SimpleSource> { new SimpleSource { Name = "SubChild2", Age = 2 } }
            }
        };
        var targets = CacharroMapper.MapList<SimpleTarget>(sources);
        Assert.AreEqual(2, targets.Count);
        Assert.AreEqual("Parent1", targets[0].Name);
        Assert.IsNotNull(targets[0].Child);
        Assert.AreEqual("Child1", targets[0].Child.Name);
        Assert.IsNotNull(targets[0].Children);
        Assert.AreEqual(1, targets[0].Children.Count);
        Assert.AreEqual("SubChild1", targets[0].Children[0].Name);
        Assert.AreEqual("Parent2", targets[1].Name);
        Assert.IsNotNull(targets[1].Child);
        Assert.AreEqual("Child2", targets[1].Child.Name);
        Assert.IsNotNull(targets[1].Children);
        Assert.AreEqual(1, targets[1].Children.Count);
        Assert.AreEqual("SubChild2", targets[1].Children[0].Name);
    }

    [TestMethod]
    public void Map_ArrayOfComplexObjectsWithNestedLists()
    {
        var source = new SourceWithArray
        {
            Name = "Root",
            Numbers = new[] { 1, 2 },
            Items = new SimpleSource[]
            {
                new SimpleSource
                {
                    Name = "Item1", Age = 10,
                    Children = new List<SimpleSource> { new SimpleSource { Name = "Nested1", Age = 100 } }
                },
                new SimpleSource
                {
                    Name = "Item2", Age = 20,
                    Children = new List<SimpleSource> { new SimpleSource { Name = "Nested2", Age = 200 } }
                }
            }
        };
        var target = CacharroMapper.Map<TargetWithArray>(source);
        Assert.AreEqual("Root", target.Name);
        Assert.IsNotNull(target.Items);
        Assert.AreEqual(2, target.Items.Length);
        Assert.AreEqual("Item1", target.Items[0].Name);
        Assert.IsNotNull(target.Items[0].Children);
        Assert.AreEqual(1, target.Items[0].Children.Count);
        Assert.AreEqual("Nested1", target.Items[0].Children[0].Name);
        Assert.AreEqual("Item2", target.Items[1].Name);
        Assert.IsNotNull(target.Items[1].Children);
        Assert.AreEqual(1, target.Items[1].Children.Count);
        Assert.AreEqual("Nested2", target.Items[1].Children[0].Name);
    }

    [TestMethod]
    public void Map_ListOfComplexObjectsWithNestedArrays()
    {
        var source = new SourceWithComplexItemList
        {
            Name = "Root",
            Items = new List<SourceItemWithTags>
            {
                new SourceItemWithTags { Name = "Item1", Age = 10, Tags = new[] { "tag1", "tag2" } },
                new SourceItemWithTags { Name = "Item2", Age = 20, Tags = new[] { "tag3" } }
            }
        };
        var target = CacharroMapper.Map<TargetWithComplexItemList>(source);
        Assert.AreEqual("Root", target.Name);
        Assert.IsNotNull(target.Items);
        Assert.AreEqual(2, target.Items.Count);
        Assert.AreEqual("Item1", target.Items[0].Name);
        Assert.IsNotNull(target.Items[0].Tags);
        Assert.AreEqual(2, target.Items[0].Tags.Length);
        Assert.AreEqual("tag1", target.Items[0].Tags[0]);
        Assert.AreEqual("tag2", target.Items[0].Tags[1]);
        Assert.AreEqual("Item2", target.Items[1].Name);
        Assert.IsNotNull(target.Items[1].Tags);
        Assert.AreEqual(1, target.Items[1].Tags.Length);
        Assert.AreEqual("tag3", target.Items[1].Tags[0]);
    }

    [TestMethod]
    public void Map_IListOfComplexObjectsWithNestedLists()
    {
        var source = new SourceWithIList
        {
            Name = "Root",
            Items = new List<SimpleSource>
            {
                new SimpleSource
                {
                    Name = "Item1", Age = 10,
                    Children = new List<SimpleSource> { new SimpleSource { Name = "NestedItem1", Age = 100 } }
                }
            }
        };
        var target = CacharroMapper.Map<TargetWithIList>(source);
        Assert.AreEqual("Root", target.Name);
        Assert.IsNotNull(target.Items);
        Assert.AreEqual(1, target.Items.Count);
        Assert.AreEqual("Item1", target.Items[0].Name);
        Assert.IsNotNull(target.Items[0].Children);
        Assert.AreEqual(1, target.Items[0].Children.Count);
        Assert.AreEqual("NestedItem1", target.Items[0].Children[0].Name);
    }

    [TestMethod]
    public void Map_DeeplyNestedObjects()
    {
        var source = new SimpleSource
        {
            Name = "Level1", Age = 1,
            Child = new SimpleSource
            {
                Name = "Level2", Age = 2,
                Child = new SimpleSource { Name = "Level3", Age = 3 },
                Children = new List<SimpleSource> { new SimpleSource { Name = "Level2Child", Age = 20 } }
            },
            Children = new List<SimpleSource>
            {
                new SimpleSource
                {
                    Name = "Level1Child", Age = 10,
                    Children = new List<SimpleSource> { new SimpleSource { Name = "Level1GrandChild", Age = 100 } }
                }
            }
        };
        var target = CacharroMapper.Map<SimpleTarget>(source);
        Assert.AreEqual("Level1", target.Name);
        Assert.AreEqual(1, target.Age);
        Assert.IsNotNull(target.Child);
        Assert.AreEqual("Level2", target.Child.Name);
        Assert.IsNotNull(target.Child.Child);
        Assert.AreEqual("Level3", target.Child.Child.Name);
        Assert.AreEqual(3, target.Child.Child.Age);
        Assert.IsNotNull(target.Child.Children);
        Assert.AreEqual(1, target.Child.Children.Count);
        Assert.AreEqual("Level2Child", target.Child.Children[0].Name);
        Assert.IsNotNull(target.Children);
        Assert.AreEqual(1, target.Children.Count);
        Assert.AreEqual("Level1Child", target.Children[0].Name);
        Assert.IsNotNull(target.Children[0].Children);
        Assert.AreEqual(1, target.Children[0].Children.Count);
        Assert.AreEqual("Level1GrandChild", target.Children[0].Children[0].Name);
    }

    [TestMethod]
    public void Map_DictionaryWithListValues()
    {
        var source = new SourceWithDictionaryOfLists
        {
            Name = "Root",
            Groups = new Dictionary<string, List<SimpleSource>>
            {
                { "Group1", new List<SimpleSource> { new SimpleSource { Name = "A", Age = 1 }, new SimpleSource { Name = "B", Age = 2 } } },
                { "Group2", new List<SimpleSource> { new SimpleSource { Name = "C", Age = 3 } } }
            }
        };
        var target = CacharroMapper.Map<TargetWithDictionaryOfLists>(source);
        Assert.AreEqual("Root", target.Name);
        Assert.IsNotNull(target.Groups);
        Assert.AreEqual(2, target.Groups.Count);
        Assert.AreEqual(2, target.Groups["Group1"].Count);
        Assert.AreEqual("A", target.Groups["Group1"][0].Name);
        Assert.AreEqual("B", target.Groups["Group1"][1].Name);
        Assert.AreEqual(1, target.Groups["Group2"].Count);
        Assert.AreEqual("C", target.Groups["Group2"][0].Name);
    }

    [TestMethod]
    public void Map_DictionaryWithArrayValues()
    {
        var source = new SourceWithDictionaryOfArrays
        {
            Name = "Root",
            Groups = new Dictionary<string, SimpleSource[]>
            {
                { "Group1", new SimpleSource[] { new SimpleSource { Name = "X", Age = 10 } } },
                { "Group2", new SimpleSource[] { new SimpleSource { Name = "Y", Age = 20 }, new SimpleSource { Name = "Z", Age = 30 } } }
            }
        };
        var target = CacharroMapper.Map<TargetWithDictionaryOfArrays>(source);
        Assert.AreEqual("Root", target.Name);
        Assert.IsNotNull(target.Groups);
        Assert.AreEqual(2, target.Groups.Count);
        Assert.AreEqual(1, target.Groups["Group1"].Length);
        Assert.AreEqual("X", target.Groups["Group1"][0].Name);
        Assert.AreEqual(2, target.Groups["Group2"].Length);
        Assert.AreEqual("Y", target.Groups["Group2"][0].Name);
        Assert.AreEqual("Z", target.Groups["Group2"][1].Name);
    }

    [TestMethod]
    public void Map_ListOfObjectsWithDictionaryProperties()
    {
        var source = new SourceWithListOfDictionaryOwners
        {
            Name = "Root",
            Items = new List<SourceWithDictionary>
            {
                new SourceWithDictionary
                {
                    Name = "Item1",
                    Properties = new Dictionary<string, string> { { "Key1", "Val1" } },
                    ComplexDictionary = new Dictionary<string, SimpleSource> { { "A", new SimpleSource { Name = "Nested", Age = 5 } } }
                }
            }
        };
        var target = CacharroMapper.Map<TargetWithListOfDictionaryOwners>(source);
        Assert.AreEqual("Root", target.Name);
        Assert.IsNotNull(target.Items);
        Assert.AreEqual(1, target.Items.Count);
        Assert.AreEqual("Item1", target.Items[0].Name);
        Assert.IsNotNull(target.Items[0].Properties);
        Assert.AreEqual("Val1", target.Items[0].Properties["Key1"]);
        Assert.IsNotNull(target.Items[0].ComplexDictionary);
        Assert.AreEqual("Nested", target.Items[0].ComplexDictionary["A"].Name);
        Assert.AreEqual(5, target.Items[0].ComplexDictionary["A"].Age);
    }
}

[TestClass]
public class CacharroMapperIndexerTests
{
    [TestMethod]
    public void Map_ObjectWithIndexerProperty_ShouldNotThrow()
    {
        var source = new SourceWithIndexer { Name = "Test" };
        var target = CacharroMapper.Map<TargetWithIndexer>(source);
        Assert.AreEqual("Test", target.Name);
    }

    [TestMethod]
    public void Map_ObjectWithIndexerAndReadOnlyIndexer_ShouldNotThrow()
    {
        var source = new SourceWithReadOnlyIndexer { Name = "ReadOnly" };
        var target = CacharroMapper.Map<SimpleTarget>(source);
        Assert.AreEqual("ReadOnly", target.Name);
    }
}

public class SourceWithIndexer
{
    public string? Name { get; set; }
    private readonly string[] _data = ["item0", "item1"];
    public string this[int index] => _data[index];
}

public class TargetWithIndexer
{
    public string? Name { get; set; }
    private readonly string[] _data = new string[10];
    public string this[int index]
    {
        get => _data[index];
        set => _data[index] = value;
    }
}

public class SourceWithReadOnlyIndexer
{
    public string? Name { get; set; }
    private readonly string[] _data = ["a", "b"];
    public string this[int index] => _data[index];
}



[TestClass]
public class CacharroMapperComplexListWithNullValuesTests
{
    [TestMethod]
    public void Map_ObjectWithComplexTypesAndNull()
    {
        var source = new List<SourceWithEnumArray>
        {
            new SourceWithEnumArray
            {
                Name = "Test",
                Statuses = null, // This should not cause an exception
                Hans=null,
                List = null
            }
        };
        var target = CacharroMapper.Map<TargetWithEnumArray>(source);
        Assert.AreEqual("Test", target.Name);
    }

    
    [TestMethod]
    public void Map_ComplexObjectWithNullValues()
    {
        var source = new List<SourcePerson>
        {
            new SourcePerson
            {
                Name = "Test",
                Id = 1,
                Age = -12, // This should not cause an exception
                Address = new SourceAddress(), // This should not cause an exception
                Tags = null, // This should not cause an exception
                Numbers = new List<string> { "one", "two" },
            }
        };
        var target = source.Map<List<TargetPerson>>();
        Assert.AreEqual(source[0].Name, target[0].Name);
    }

    public class SourcePerson
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int Age { get; set; }
        public SourceAddress Address { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Numbers { get; set; }
    }
    public class TargetPerson
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int Age { get; set; }
        public TargetAddress Address { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Numbers { get; set; }
    }

    public class SourceAddress
    {
        public string? Street { get; set; }
        public string? City { get; set; }
    }
    public class TargetAddress
    {
        public string? Street { get; set; }
        public string? City { get; set; }
    }
}

// Models for cross-collection mapping tests
public class SourceWithIntArrayProp
{
    public string? Name { get; set; }
    public int[]? Numbers { get; set; }
    public SimpleSource[]? Items { get; set; }
}

public class TargetWithIntListProp
{
    public string? Name { get; set; }
    public List<int>? Numbers { get; set; }
    public List<SimpleTarget>? Items { get; set; }
}

public class SourceWithIntListProp
{
    public string? Name { get; set; }
    public List<int>? Numbers { get; set; }
    public List<SimpleSource>? Items { get; set; }
}

public class TargetWithIntArrayProp
{
    public string? Name { get; set; }
    public int[]? Numbers { get; set; }
    public SimpleTarget[]? Items { get; set; }
}

public class SourceWithHashSetProp
{
    public string? Name { get; set; }
    public HashSet<string>? Tags { get; set; }
    public HashSet<int>? Numbers { get; set; }
    public HashSet<SimpleSource>? Items { get; set; }
}

public class TargetWithHashSetProp
{
    public string? Name { get; set; }
    public HashSet<string>? Tags { get; set; }
    public HashSet<int>? Numbers { get; set; }
    public HashSet<SimpleTarget>? Items { get; set; }
}

public class TargetWithArrayFromHashSet
{
    public string? Name { get; set; }
    public string[]? Tags { get; set; }
    public int[]? Numbers { get; set; }
}

public class TargetWithListFromHashSet
{
    public string? Name { get; set; }
    public List<string>? Tags { get; set; }
    public List<int>? Numbers { get; set; }
}

public class SourceWithStringArrayProp
{
    public string? Name { get; set; }
    public string[]? Tags { get; set; }
}

public class TargetWithHashSetFromArray
{
    public string? Name { get; set; }
    public HashSet<string>? Tags { get; set; }
}

public class TargetWithHashSetFromList
{
    public string? Name { get; set; }
    public HashSet<string>? Tags { get; set; }
}

public class SourceWithStringListProp
{
    public string? Name { get; set; }
    public List<string>? Tags { get; set; }
}

[TestClass]
public class CacharroMapperCrossCollectionTests
{
    // Array → List

    [TestMethod]
    public void Map_ArrayToList_Primitives()
    {
        var source = new SourceWithIntArrayProp { Name = "Test", Numbers = new[] { 1, 2, 3 } };
        var target = CacharroMapper.Map<TargetWithIntListProp>(source);
        Assert.AreEqual("Test", target.Name);
        Assert.IsNotNull(target.Numbers);
        Assert.AreEqual(3, target.Numbers.Count);
        Assert.AreEqual(1, target.Numbers[0]);
        Assert.AreEqual(2, target.Numbers[1]);
        Assert.AreEqual(3, target.Numbers[2]);
    }

    [TestMethod]
    public void Map_ArrayToList_ComplexObjects()
    {
        var source = new SourceWithIntArrayProp
        {
            Name = "Test",
            Numbers = new[] { 10 },
            Items = new SimpleSource[]
            {
                new SimpleSource { Name = "A", Age = 1 },
                new SimpleSource { Name = "B", Age = 2 }
            }
        };
        var target = CacharroMapper.Map<TargetWithIntListProp>(source);
        Assert.IsNotNull(target.Items);
        Assert.AreEqual(2, target.Items.Count);
        Assert.AreEqual("A", target.Items[0].Name);
        Assert.AreEqual(1, target.Items[0].Age);
        Assert.AreEqual("B", target.Items[1].Name);
        Assert.AreEqual(2, target.Items[1].Age);
    }

    [TestMethod]
    public void Map_ArrayToList_Empty()
    {
        var source = new SourceWithIntArrayProp { Name = "Empty", Numbers = new int[0] };
        var target = CacharroMapper.Map<TargetWithIntListProp>(source);
        Assert.IsNotNull(target.Numbers);
        Assert.AreEqual(0, target.Numbers.Count);
    }

    // List → Array

    [TestMethod]
    public void Map_ListToArray_Primitives()
    {
        var source = new SourceWithIntListProp { Name = "Test", Numbers = new List<int> { 4, 5, 6 } };
        var target = CacharroMapper.Map<TargetWithIntArrayProp>(source);
        Assert.AreEqual("Test", target.Name);
        Assert.IsNotNull(target.Numbers);
        Assert.AreEqual(3, target.Numbers.Length);
        Assert.AreEqual(4, target.Numbers[0]);
        Assert.AreEqual(5, target.Numbers[1]);
        Assert.AreEqual(6, target.Numbers[2]);
    }

    [TestMethod]
    public void Map_ListToArray_ComplexObjects()
    {
        var source = new SourceWithIntListProp
        {
            Name = "Test",
            Numbers = new List<int> { 1 },
            Items = new List<SimpleSource>
            {
                new SimpleSource { Name = "X", Age = 10 },
                new SimpleSource { Name = "Y", Age = 20 }
            }
        };
        var target = CacharroMapper.Map<TargetWithIntArrayProp>(source);
        Assert.IsNotNull(target.Items);
        Assert.AreEqual(2, target.Items.Length);
        Assert.AreEqual("X", target.Items[0].Name);
        Assert.AreEqual(10, target.Items[0].Age);
        Assert.AreEqual("Y", target.Items[1].Name);
        Assert.AreEqual(20, target.Items[1].Age);
    }

    [TestMethod]
    public void Map_ListToArray_Empty()
    {
        var source = new SourceWithIntListProp { Name = "Empty", Numbers = new List<int>() };
        var target = CacharroMapper.Map<TargetWithIntArrayProp>(source);
        Assert.IsNotNull(target.Numbers);
        Assert.AreEqual(0, target.Numbers.Length);
    }

    // HashSet → Array

    [TestMethod]
    public void Map_HashSetToArray_Primitives()
    {
        var source = new SourceWithHashSetProp { Name = "Test", Tags = new HashSet<string> { "a", "b", "c" } };
        var target = CacharroMapper.Map<TargetWithArrayFromHashSet>(source);
        Assert.AreEqual("Test", target.Name);
        Assert.IsNotNull(target.Tags);
        Assert.AreEqual(3, target.Tags.Length);
        CollectionAssert.AreEquivalent(new[] { "a", "b", "c" }, target.Tags);
    }

    [TestMethod]
    public void Map_HashSetToArray_Empty()
    {
        var source = new SourceWithHashSetProp { Name = "Empty", Tags = new HashSet<string>() };
        var target = CacharroMapper.Map<TargetWithArrayFromHashSet>(source);
        Assert.IsNotNull(target.Tags);
        Assert.AreEqual(0, target.Tags.Length);
    }

    // Array → HashSet

    [TestMethod]
    public void Map_ArrayToHashSet_Primitives()
    {
        var source = new SourceWithStringArrayProp { Name = "Test", Tags = new[] { "x", "y", "z" } };
        var target = CacharroMapper.Map<TargetWithHashSetFromArray>(source);
        Assert.AreEqual("Test", target.Name);
        Assert.IsNotNull(target.Tags);
        Assert.AreEqual(3, target.Tags.Count);
        Assert.IsTrue(target.Tags.Contains("x"));
        Assert.IsTrue(target.Tags.Contains("y"));
        Assert.IsTrue(target.Tags.Contains("z"));
    }

    [TestMethod]
    public void Map_ArrayToHashSet_Empty()
    {
        var source = new SourceWithStringArrayProp { Name = "Empty", Tags = new string[0] };
        var target = CacharroMapper.Map<TargetWithHashSetFromArray>(source);
        Assert.IsNotNull(target.Tags);
        Assert.AreEqual(0, target.Tags.Count);
    }

    // HashSet → List

    [TestMethod]
    public void Map_HashSetToList_Primitives()
    {
        var source = new SourceWithHashSetProp { Name = "Test", Tags = new HashSet<string> { "one", "two" } };
        var target = CacharroMapper.Map<TargetWithListFromHashSet>(source);
        Assert.AreEqual("Test", target.Name);
        Assert.IsNotNull(target.Tags);
        Assert.AreEqual(2, target.Tags.Count);
        CollectionAssert.AreEquivalent(new[] { "one", "two" }, target.Tags);
    }

    [TestMethod]
    public void Map_HashSetToList_Empty()
    {
        var source = new SourceWithHashSetProp { Name = "Empty", Tags = new HashSet<string>() };
        var target = CacharroMapper.Map<TargetWithListFromHashSet>(source);
        Assert.IsNotNull(target.Tags);
        Assert.AreEqual(0, target.Tags.Count);
    }

    // List → HashSet

    [TestMethod]
    public void Map_ListToHashSet_Primitives()
    {
        var source = new SourceWithStringListProp { Name = "Test", Tags = new List<string> { "foo", "bar" } };
        var target = CacharroMapper.Map<TargetWithHashSetFromList>(source);
        Assert.AreEqual("Test", target.Name);
        Assert.IsNotNull(target.Tags);
        Assert.AreEqual(2, target.Tags.Count);
        Assert.IsTrue(target.Tags.Contains("foo"));
        Assert.IsTrue(target.Tags.Contains("bar"));
    }

    [TestMethod]
    public void Map_ListToHashSet_Empty()
    {
        var source = new SourceWithStringListProp { Name = "Empty", Tags = new List<string>() };
        var target = CacharroMapper.Map<TargetWithHashSetFromList>(source);
        Assert.IsNotNull(target.Tags);
        Assert.AreEqual(0, target.Tags.Count);
    }

    // Direct Map<T> calls where T is a collection type

    [TestMethod]
    public void Map_DirectArrayToList()
    {
        int[] sourceArr = new[] { 10, 20, 30 };
        var result = CacharroMapper.Map<List<int>>(sourceArr);
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(10, result[0]);
        Assert.AreEqual(20, result[1]);
        Assert.AreEqual(30, result[2]);
    }

    [TestMethod]
    public void Map_DirectListToArray()
    {
        var sourceList = new List<string> { "alpha", "beta" };
        var result = CacharroMapper.Map<string[]>(sourceList);
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual("alpha", result[0]);
        Assert.AreEqual("beta", result[1]);
    }

    [TestMethod]
    public void Map_DirectListToHashSet()
    {
        var sourceList = new List<string> { "p", "q", "r" };
        var result = CacharroMapper.Map<HashSet<string>>(sourceList);
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count);
        Assert.IsTrue(result.Contains("p"));
        Assert.IsTrue(result.Contains("q"));
        Assert.IsTrue(result.Contains("r"));
    }

    [TestMethod]
    public void Map_DirectHashSetToArray()
    {
        var sourceSet = new HashSet<int> { 7, 8, 9 };
        var result = CacharroMapper.Map<int[]>(sourceSet);
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Length);
        CollectionAssert.AreEquivalent(new[] { 7, 8, 9 }, result);
    }

    [TestMethod]
    public void Map_DirectHashSetToList()
    {
        var sourceSet = new HashSet<int> { 1, 2 };
        var result = CacharroMapper.Map<List<int>>(sourceSet);
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        CollectionAssert.AreEquivalent(new[] { 1, 2 }, result);
    }

    [TestMethod]
    public void Map_DirectArrayToHashSet()
    {
        string[] sourceArr = new[] { "m", "n" };
        var result = CacharroMapper.Map<HashSet<string>>(sourceArr);
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Contains("m"));
        Assert.IsTrue(result.Contains("n"));
    }
}