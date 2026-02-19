using System;
using System.Reflection;

namespace CacharroMapper.CacharroMapperExtension
{
    public static class CacharroMapperExtension
    {
        public static TTarget Map<TTarget>(this object source) where TTarget : new()
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return CacharroMapper.Map<TTarget>(source);
        }
    }
}