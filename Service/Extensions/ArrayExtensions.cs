using System;
using System.ComponentModel;

namespace DataAnalysis.Service.Extensions
{
    public static class ArrayExtensions
    {
        public static TType Get<TType>(this string[] array, object tEnum)
        {
            if (array == null)
            {
                throw new ArgumentException("array");
            }

            if (!(tEnum is Enum))
            {
                throw new ArgumentException("tEnum");
            }

            var index = ((Enum)tEnum).GetHashCode() - 1;
            var value = array[index];

            if (value == null)
            {
                return default(TType);
            }

            var converter = TypeDescriptor.GetConverter(typeof(TType));
            var valueType = value.GetType();

            if (!converter.CanConvertFrom(valueType))
            {
                return default(TType);
            }

            try
            {
                var result = (TType)converter.ConvertFrom(value);
                return result;
            }
            catch
            {
                // ignored
            }

            return default(TType);
        }
    }
}