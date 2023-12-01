using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Ankom.Common
{
    public static class EnumExtensions
    {
        public static IDictionary<string, int> ToDictionary(this Type enumType)
        {
            return Enum.GetValues(enumType)
            .Cast<object>()
            .ToDictionary(v => ((Enum)v).ToEnumDescription(), k => (int)k);
        }

        public static string ToEnumDescription(this Enum en) //ext method
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return en.ToString();
        }

        // This extension method is broken out so you can use a similar pattern with 
        // other MetaData elements in the future. This is your base method for each.
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            try
            {
                var type = value.GetType();
                var memberInfo = type.GetMember(value.ToString());
                var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
                return attributes.Length > 0
                  ? (T)attributes[0]
                  : null;
            }
            catch
            {
                return null;
            }
        }

        // This method creates a specific call to the above method, requesting the
        // Description MetaData attribute.
        public static string ToName(this Enum value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

    }


}
