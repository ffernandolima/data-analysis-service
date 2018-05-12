using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DataAnalysis.Framework.Extensions
{
    public static class EnumExtensions
    {
        public static TEnum GetValueFromDescription<TEnum>(string description) where TEnum : struct
        {
            var type = typeof(TEnum);

            if (type.IsEnum)
            {
                foreach (var field in type.GetFields())
                {
                    var attribute = (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute);

                    if (attribute != null)
                    {
                        if (attribute.Description.Equals(description, StringComparison.OrdinalIgnoreCase))
                        {
                            return (TEnum)field.GetValue(null);
                        }
                    }
                    else
                    {
                        if (field.Name.Equals(description, StringComparison.OrdinalIgnoreCase))
                        {
                            return (TEnum)field.GetValue(null);
                        }
                    }
                }
            }

            return default(TEnum);
        }

        public static string GetDescriptionFromValue<TEnum>(this TEnum tenum) where TEnum : struct
        {
            var type = typeof(TEnum);

            if (type.IsEnum)
            {
                var memInfo = type.GetMember(tenum.ToString());

                if (memInfo != null && memInfo.Length > 0)
                {
                    var attrs = memInfo.First().GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (attrs != null && attrs.Length > 0)
                    {
                        return ((DescriptionAttribute)attrs.First()).Description;
                    }
                }
            }

            return tenum.ToString();
        }

        public static IEnumerable<string> GetAllDescriptions<TEnum>(bool ignoreNone = false) where TEnum : struct
        {
            var type = typeof(TEnum);

            if (!type.IsEnum)
            {
                return Enumerable.Empty<string>();
            }

            var extensions = new List<string>();

            foreach (var value in Enum.GetValues(type))
            {
                var descr = value.ToString();
                var memInfo = type.GetMember(descr);

                if (memInfo != null && memInfo.Length > 0)
                {
                    var attrs = memInfo.First().GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (attrs != null && attrs.Length > 0)
                    {
                        descr = ((DescriptionAttribute)attrs.First()).Description;
                    }
                }

                if (ignoreNone && (descr.Equals("None", StringComparison.OrdinalIgnoreCase) || value.GetHashCode() == 0))
                {
                    continue;
                }

                extensions.Add(descr);
            }

            return extensions;
        }
    }
}
