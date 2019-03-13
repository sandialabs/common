using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace gov.sandia.sld.common.utilities
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Apply the [Description("Extended Description")] attribute to the
        /// enum values you'd like to change
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            MemberInfo[] memInfo = type.GetMember(value.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return value.ToString();
        }

        /// <summary>
        /// Got this little gem from
        /// http://codereview.stackexchange.com/a/5354
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name) // I prefer to get attributes this way
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }

        /// <summary>
        /// Get all the values of the specified Enum
        /// Found at: http://stackoverflow.com/a/972323/706747
        /// </summary>
        /// <typeparam name="T">The type of the Enum</typeparam>
        /// <returns>A list of all the types</returns>
        public static IEnumerable<T> GetValues<T>() where T : struct, IConvertible
        {
            return (T[])Enum.GetValues(typeof(T));
        }
    }
}
