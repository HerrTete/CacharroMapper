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

        // Handle generic collections (List<T>, IEnumerable<T>, arrays, etc.)
        if (IsEnumerableType(targetType, out var targetElementType) && 
            IsEnumerableType(valueType, out var sourceElementType))
        {
            return MapCollection(value, targetType, targetElementType!);
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

    private static bool IsEnumerableType(Type type, out Type? elementType)
    {
        elementType = null;

        // Check if it's an array
        if (type.IsArray)
        {
            elementType = type.GetElementType();
            return elementType != null;
        }

        // Check if it's a generic type
        if (type.IsGenericType)
        {
            var genericTypeDef = type.GetGenericTypeDefinition();
            if (genericTypeDef == typeof(List<>) ||
                genericTypeDef == typeof(IEnumerable<>) ||
                genericTypeDef == typeof(ICollection<>) ||
                genericTypeDef == typeof(IList<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
        }

        // Check if it implements IEnumerable<T>
        var enumerableInterface = type.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && 
                                i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        
        if (enumerableInterface != null)
        {
            elementType = enumerableInterface.GetGenericArguments()[0];
            return true;
        }

        return false;
    }

    private static object MapCollection(object sourceCollection, Type targetType, Type targetElementType)
    {
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
