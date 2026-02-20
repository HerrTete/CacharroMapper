using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CacharroMapper;

public class CacharroMapper
{
    public static List<T> MapList<T>(IEnumerable<object> source, List<PropertyNameMapping>? propertyNameMappings = null) where T : new()
    {
        var result = new List<T>();
        foreach (var item in source)
        {
            if (item == null)
            {
                result.Add(default!);
            }
            else
            {
                result.Add(Map<T>(item, propertyNameMappings));
            }
        }
        return result;
    }

    public static T Map<T>(object source, List<PropertyNameMapping>? propertyNameMappings = null)
    {
        var sourceType = source.GetType();
        var targetType = typeof(T);

        // If source is a collection and target is also a collection, map the collection
        if (IsAnyCollection(sourceType) && IsAnyCollection(targetType))
        {
            return (T)MapCollectionToCollection(source, targetType, propertyNameMappings);
        }

        var target = (T)Activator.CreateInstance(typeof(T))!;

        // If source is a list/collection but target is not, map the first element
        if (IsGenericList(sourceType))
        {
            var rawEnumerator = ((IEnumerable)source).GetEnumerator();
            try
            {
                if (!rawEnumerator.MoveNext() || rawEnumerator.Current == null) return target;
                source = rawEnumerator.Current;
            }
            finally
            {
                (rawEnumerator as IDisposable)?.Dispose();
            }
            sourceType = source.GetType();
        }

        // Convert property name mappings to dictionary for O(1) lookups
        Dictionary<string, string>? mappingDict = null;
        if (propertyNameMappings != null && propertyNameMappings.Count > 0)
        {
            mappingDict = propertyNameMappings.ToDictionary(m => m.TargetPropertyName, m => m.SourcePropertyName);
        }

        foreach (var targetProp in targetType.GetProperties())
        {
            // Check if there's a property name mapping for this target property
            var sourcePropertyName = targetProp.Name;
            if (mappingDict != null && mappingDict.TryGetValue(targetProp.Name, out var mappedName))
            {
                sourcePropertyName = mappedName;
            }

            var sourceProp = sourceType.GetProperty(sourcePropertyName);
            if (sourceProp != null && sourceProp.CanRead && targetProp.CanWrite
                && sourceProp.GetIndexParameters().Length == 0
                && targetProp.GetIndexParameters().Length == 0)
            {
                var value = sourceProp.GetValue(source);
                var mappedValue = MapValue(value, targetProp.PropertyType, propertyNameMappings);
                targetProp.SetValue(target, mappedValue);
            }
        }

        return target;
    }

    private static object? MapValue(object? value, Type targetType, List<PropertyNameMapping>? propertyNameMappings = null)
    {
        if (value == null)
        {
            return null;
        }

        var sourceType = value.GetType();

        // Handle enum mapping by name
        if (sourceType.IsEnum && targetType.IsEnum)
        {
            var enumName = value.ToString() ?? string.Empty;
            if (Enum.IsDefined(targetType, enumName))
            {
                return Enum.Parse(targetType, enumName);
            }
            // If the enum name doesn't exist in target, return the default value
            return Activator.CreateInstance(targetType)!;
        }

        // Handle collection types (arrays, lists, hashsets, etc.) with cross-collection support
        if (IsAnyCollection(targetType) && IsAnyCollection(sourceType))
        {
            return MapCollectionToCollection(value, targetType, propertyNameMappings);
        }

        // Handle Dictionary<TKey, TValue> (must be before IsAssignableFrom check)
        if (IsGenericDictionary(targetType) && IsGenericDictionary(sourceType))
        {
            return MapDictionary(value, targetType);
        }

        // If types match or target type is assignable from source type (for primitive types and strings)
        if (targetType.IsAssignableFrom(sourceType))
        {
            return value;
        }

        // Handle complex objects (nested mapping)
        if (targetType.IsClass && targetType != typeof(string))
        {
            var mapMethod = typeof(CacharroMapper).GetMethod(nameof(Map))!.MakeGenericMethod(targetType);
            return mapMethod.Invoke(null, new object?[] { value, propertyNameMappings });
        }

        return value;
    }

    private static object MapArray(object sourceArray, Type targetType, List<PropertyNameMapping>? propertyNameMappings = null)
    {
        var sourceArr = (Array)sourceArray;
        var targetElementType = targetType.GetElementType()!;
        var targetArr = Array.CreateInstance(targetElementType, sourceArr.Length);

        for (int i = 0; i < sourceArr.Length; i++)
        {
            var sourceElement = sourceArr.GetValue(i);
            var mappedElement = MapValue(sourceElement, targetElementType, propertyNameMappings);
            targetArr.SetValue(mappedElement, i);
        }

        return targetArr;
    }

    private static object MapList(object sourceList, Type targetType, List<PropertyNameMapping>? propertyNameMappings = null)
    {
        var sourceEnumerable = (IEnumerable)sourceList;
        var targetGenericArgs = targetType.GetGenericArguments();
        var targetElementType = targetGenericArgs[0];

        // Create the target list
        Type listType;
        if (targetType.IsInterface)
        {
            // If target is IList<T>, create a List<T>
            listType = typeof(List<>).MakeGenericType(targetElementType);
        }
        else
        {
            listType = targetType;
        }

        var targetList = (IList)Activator.CreateInstance(listType)!;

        foreach (var sourceElement in sourceEnumerable)
        {
            var mappedElement = MapValue(sourceElement, targetElementType, propertyNameMappings);
            targetList.Add(mappedElement);
        }

        return targetList;
    }

    private static object MapDictionary(object sourceDictionary, Type targetType, List<PropertyNameMapping>? propertyNameMappings = null)
    {
        var sourceDict = (IDictionary)sourceDictionary;
        var targetGenericArgs = targetType.GetGenericArguments();
        var targetKeyType = targetGenericArgs[0];
        var targetValueType = targetGenericArgs[1];

        // Create the target dictionary
        var dictType = typeof(Dictionary<,>).MakeGenericType(targetKeyType, targetValueType);
        var targetDict = (IDictionary)Activator.CreateInstance(dictType)!;

        foreach (DictionaryEntry entry in sourceDict)
        {
            var mappedKey = MapValue(entry.Key, targetKeyType, propertyNameMappings);
            var mappedValue = MapValue(entry.Value, targetValueType, propertyNameMappings);
            targetDict.Add(mappedKey!, mappedValue);
        }

        return targetDict;
    }

    private static object MapCollectionToCollection(object source, Type targetType, List<PropertyNameMapping>? propertyNameMappings = null)
    {
        var sourceEnumerable = (IEnumerable)source;
        var targetElementType = GetCollectionElementType(targetType);

        var mappedElements = new List<object?>();
        foreach (var element in sourceEnumerable)
        {
            mappedElements.Add(MapValue(element, targetElementType, propertyNameMappings));
        }

        if (targetType.IsArray)
        {
            var arr = Array.CreateInstance(targetElementType, mappedElements.Count);
            for (int i = 0; i < mappedElements.Count; i++)
                arr.SetValue(mappedElements[i], i);
            return arr;
        }

        var genericTypeDef = targetType.IsGenericType ? targetType.GetGenericTypeDefinition() : null;

        if (genericTypeDef == typeof(HashSet<>) || genericTypeDef == typeof(ISet<>))
        {
            var hashSetType = typeof(HashSet<>).MakeGenericType(targetElementType);
            var set = Activator.CreateInstance(hashSetType)!;
            var addMethod = hashSetType.GetMethod("Add")!;
            foreach (var element in mappedElements)
                addMethod.Invoke(set, new[] { element });
            return set;
        }

        // Default: List<T> for List<>, IList<>, ICollection<>, IEnumerable<>
        var listType = targetType.IsInterface
            ? typeof(List<>).MakeGenericType(targetElementType)
            : targetType;
        var list = (IList)Activator.CreateInstance(listType)!;
        foreach (var element in mappedElements)
            list.Add(element);
        return list;
    }

    private static T MapListToList<T>(object sourceList, Type targetType, List<PropertyNameMapping>? propertyNameMappings) where T : new()
    {
        var sourceEnumerable = (IEnumerable)sourceList;
        var targetGenericArgs = targetType.GetGenericArguments();
        var targetElementType = targetGenericArgs[0];

        // Create the target list
        Type listType = typeof(List<>).MakeGenericType(targetElementType);
        var targetList = (IList)Activator.CreateInstance(listType)!;

        foreach (var sourceElement in sourceEnumerable)
        {
            var mappedElement = MapValue(sourceElement, targetElementType, propertyNameMappings);
            targetList.Add(mappedElement);
        }

        return (T)(object)targetList;
    }

    private static bool IsGenericList(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        var genericTypeDef = type.GetGenericTypeDefinition();
        return genericTypeDef == typeof(List<>) || 
               genericTypeDef == typeof(IList<>);
    }

    private static bool IsGenericDictionary(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        var genericTypeDef = type.GetGenericTypeDefinition();
        return genericTypeDef == typeof(Dictionary<,>) || 
               genericTypeDef == typeof(IDictionary<,>);
    }

    private static bool IsAnyCollection(Type type)
    {
        if (type.IsArray) return true;
        if (!type.IsGenericType) return false;
        var genericTypeDef = type.GetGenericTypeDefinition();
        return genericTypeDef == typeof(List<>) ||
               genericTypeDef == typeof(IList<>) ||
               genericTypeDef == typeof(ICollection<>) ||
               genericTypeDef == typeof(IEnumerable<>) ||
               genericTypeDef == typeof(HashSet<>) ||
               genericTypeDef == typeof(ISet<>);
    }

    private static Type GetCollectionElementType(Type type)
    {
        if (type.IsArray) return type.GetElementType()!;
        return type.GetGenericArguments()[0];
    }
}
