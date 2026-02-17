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
            return null;

        var valueType = value.GetType();

        // If types match or target is object type, return as is
        if (targetType.IsAssignableFrom(valueType))
            return value;

        // Handle generic collections (List<T>, IEnumerable<T>, etc.)
        if (IsGenericCollection(targetType) && IsGenericCollection(valueType))
        {
            return MapCollection(value, targetType);
        }

        // Handle complex objects (nested mapping)
        if (targetType.IsClass && targetType != typeof(string) && HasParameterlessConstructor(targetType))
        {
            var mapMethod = typeof(EasyMapper).GetMethod(nameof(Map));
            var genericMapMethod = mapMethod!.MakeGenericMethod(targetType);
            return genericMapMethod.Invoke(null, new[] { value });
        }

        return value;
    }

    private static bool IsGenericCollection(Type type)
    {
        return type.IsGenericType && 
               (type.GetGenericTypeDefinition() == typeof(List<>) ||
                type.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                type.GetGenericTypeDefinition() == typeof(ICollection<>));
    }

    private static object MapCollection(object sourceCollection, Type targetType)
    {
        var targetElementType = targetType.GetGenericArguments()[0];
        var listType = typeof(List<>).MakeGenericType(targetElementType);
        var targetList = (System.Collections.IList)Activator.CreateInstance(listType)!;

        foreach (var item in (System.Collections.IEnumerable)sourceCollection)
        {
            var mappedItem = MapValue(item, targetElementType);
            targetList.Add(mappedItem);
        }

        return targetList;
    }

    private static bool HasParameterlessConstructor(Type type)
    {
        return type.GetConstructor(Type.EmptyTypes) != null;
    }

}
