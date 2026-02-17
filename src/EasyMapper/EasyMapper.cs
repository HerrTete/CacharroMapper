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
                targetProp.SetValue(target, value);
            }
        }

        return target;
    }

}
