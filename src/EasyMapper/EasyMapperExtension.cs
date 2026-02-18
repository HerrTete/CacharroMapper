using System;
using System.Reflection;

namespace EasyMapper.EasyMapperExtension
{
    public static class EasyMapperExtension
    {
        public static TTarget Map<TTarget>(this object source) where TTarget : new()
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return EasyMapper.Map<TTarget>(source);
        }
    }
}