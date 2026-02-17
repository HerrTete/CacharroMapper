using System.Collections;

namespace EasyMapper;

public class EasyMapper
{
    public static T Map<T>(object source) where T : new()
    {
        var target = new T();
        var sourceType = source.GetType();
        var targetType = typeof(T);

        foreach (var targetProp in targetType.GetProperties())
        {
            var sourceProp = sourceType.GetProperty(targetProp.Name);
            if (sourceProp != null && sourceProp.CanRead && targetProp.CanWrite)
            {
                var value = sourceProp.GetValue(source);
                var mappedValue = MapValue(value, targetProp.PropertyType);
                targetProp.SetValue(target, mappedValue);
            }
        }

        return target;
    }

    private static object? MapValue(object? value, Type targetType)
    {
        if (value == null)
        {
            return null;
        }

        var sourceType = value.GetType();

        // If types match or target type is assignable from source type (for primitive types and strings)
        if (targetType.IsAssignableFrom(sourceType))
        {
            return value;
        }

        // Handle arrays
        if (targetType.IsArray && sourceType.IsArray)
        {
            return MapArray(value, targetType);
        }

        // Handle IList<T> and List<T>
        if (IsGenericList(targetType) && IsGenericList(sourceType))
        {
            return MapList(value, targetType);
        }

        // Handle Dictionary<TKey, TValue>
        if (IsGenericDictionary(targetType) && IsGenericDictionary(sourceType))
        {
            return MapDictionary(value, targetType);
        }

        // Handle complex objects (nested mapping)
        if (targetType.IsClass && targetType != typeof(string))
        {
            var mapMethod = typeof(EasyMapper).GetMethod(nameof(Map))!.MakeGenericMethod(targetType);
            return mapMethod.Invoke(null, new[] { value });
        }

        return value;
    }

    private static object MapArray(object sourceArray, Type targetType)
    {
        var sourceArr = (Array)sourceArray;
        var targetElementType = targetType.GetElementType()!;
        var targetArr = Array.CreateInstance(targetElementType, sourceArr.Length);

        for (int i = 0; i < sourceArr.Length; i++)
        {
            var sourceElement = sourceArr.GetValue(i);
            var mappedElement = MapValue(sourceElement, targetElementType);
            targetArr.SetValue(mappedElement, i);
        }

        return targetArr;
    }

    private static object MapList(object sourceList, Type targetType)
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
            var mappedElement = MapValue(sourceElement, targetElementType);
            targetList.Add(mappedElement);
        }

        return targetList;
    }

    private static object MapDictionary(object sourceDictionary, Type targetType)
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
            var mappedKey = MapValue(entry.Key, targetKeyType);
            var mappedValue = MapValue(entry.Value, targetValueType);
            targetDict.Add(mappedKey!, mappedValue);
        }

        return targetDict;
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
}
