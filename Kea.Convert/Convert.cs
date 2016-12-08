using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Kea
{
    /// <summary>
    /// Type conversion helper
    /// </summary>
    public static class ConvertHelper
    {
        /// <summary>
        /// Cast an object to a given type
        /// </summary>
        /// <param name="type">Type to cast to</param>
        /// <param name="obj">Object to cast</param>
        /// <returns></returns>
        public static object Cast(Type type, object obj)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (obj == null)
            {
                if (Nullable.GetUnderlyingType(type) != null || !type.IsValueType)
                {
                    return null;
                }
                else
                {
                    throw new ArgumentNullException("obj", $"obj can't be null since type '{type}' doesn't accept nulls");
                }
            }

            var DataParam = Expression.Parameter(typeof(object));
            var Body = Expression.Block(Expression.Convert(Expression.Convert(DataParam, obj.GetType()), type));

            var Run = Expression.Lambda(Body, DataParam).Compile();
            var ret = Run.DynamicInvoke(obj);
            return ret;
        }

        /// <summary>
        /// Convert a string to a given type using the current culture as a format provider
        /// </summary>
        /// <param name="s">The string to convert</param>
        /// <param name="type">The desired type</param>
        /// <returns></returns>
        public static object ConvertFromString(string s, Type type)
        {
            return ConvertFromString(s, type, CultureInfo.CurrentCulture);
        }
        /// <summary>
        /// Converts a string to a given type
        /// </summary>
        /// <exception cref="ArgumentException" />
        /// <exception cref="FormatException" />
        /// <param name="s">The string to convert</param>
        /// <param name="type">The desired type</param>
        /// <returns></returns>
        public static object ConvertFromString(string s, Type type, IFormatProvider formatProvider)
        {
            if (type == typeof(string))
                return s;

            var nullable = (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
            if (string.IsNullOrEmpty(s) && (!type.IsValueType || nullable))
            {
                return null;
            }
            else
            {
                if (s == null && !nullable)
                    throw new FormatException($"String is null but type '{type.Name}' doesn't accept nulls");

                var nonNullableType = nullable ? type.GenericTypeArguments[0] : type;

                object value;

                var parseMethods =
                    nonNullableType
                    .GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                    .Where(x => x.Name == "Parse" && x.IsStatic);
                //Parse(string, IFormatProvider) method
                var parseWithFormatProviderMethod = parseMethods
                    .Where(x =>
                        x.GetParameters()
                        .Select(y => y.ParameterType).SequenceEqual(new[] { typeof(string), typeof(IFormatProvider) }))
                    .FirstOrDefault();

                //Parse(string) method
                var parseMethod = parseMethods
                    .Where(x =>
                        x.GetParameters()
                        .Select(y => y.ParameterType).SequenceEqual(new[] { typeof(string) }))
                    .FirstOrDefault();

                try
                {
                    if (nonNullableType == typeof(bool))
                    {
                        value = s == "1" ? true :
                                s == "0" ? false :
                                bool.Parse(s);
                    }
                    else if (parseWithFormatProviderMethod != null)
                    {
                        value = parseWithFormatProviderMethod.Invoke(null, new object[] { s, formatProvider });
                    }
                    else if (parseMethod != null)
                    {
                        value = parseMethod.Invoke(null, new object[] { s });
                    }
                    else
                    {
                        throw new ArgumentException("Tipo no soportado");
                    }
                }
                catch (OverflowException)
                {

                    throw new FormatException();
                }


                if (nullable)
                {
                    return Cast(typeof(Nullable<>).MakeGenericType(nonNullableType), value);
                }
                else
                    return value;
            }
        }
    }
}
