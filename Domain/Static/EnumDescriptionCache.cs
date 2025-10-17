using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;

namespace Domain.Static
{
    public static class EnumDescriptionCache<TEnum> where TEnum : Enum
    {
        public static readonly string[] DescriptionsArray = GetDescriptionArray();
        public static readonly ImmutableDictionary<string, TEnum> DescriptionToEnum = GetDescriptionDictionary();

        private static string[] GetDescriptionArray()
        {
            var type = typeof(TEnum);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            var list = new List<(int intValue, string description)>(fields.Length);

            foreach (var field in fields)
            {
                var value = (TEnum)field.GetValue(null);
                var intValue = Convert.ToInt32(value);
                var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
                list.Add((intValue, attribute?.Description ?? value.ToString()));
            }

            list.Sort((a, b) => a.intValue.CompareTo(b.intValue));
            return [.. list.Select(x => x.description)];
        }

        private static ImmutableDictionary<string, TEnum> GetDescriptionDictionary()
        {
            var type = typeof(TEnum);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            var result = ImmutableDictionary.CreateBuilder<string, TEnum>(StringComparer.OrdinalIgnoreCase);

            foreach (var field in fields)
            {
                var value = (TEnum)field.GetValue(null);
                var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
                var desc = attribute?.Description ?? value.ToString();
                result[desc] = value;
            }

            return result.ToImmutable();
        }
    }
}
