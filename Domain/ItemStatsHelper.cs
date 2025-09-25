using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Domain
{
    /// <summary>
    /// Helper that exposes ItemStats property metadata (description + accessor) in a cached,
    /// efficient form suitable for building UI columns or mapping to dictionaries.
    /// Also declares <see cref="StatColumnAttribute"/> which can be applied to properties
    /// in <see cref="ItemStats"/> to control ordering (optional).
    /// </summary>
    public static class ItemStatsHelper
    {
        // Descriptor exposed to callers
        public sealed class StatDescriptor
        {
            public PropertyInfo Property { get; init; }
            public string PropertyName { get; init; }
            public string Header { get; init; }
            public int Order { get; init; }
            public Func<ItemStats, object> Getter { get; init; }
            public Type PropertyType { get; init; }
        }

        // Optional attribute you can add to ItemStats properties to control order (lower first).
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
        public sealed class StatColumnAttribute(int order) : Attribute
        {
            public int Order { get; } = order;
        }

        // Cached descriptors for ItemStats properties (built once).
        private static readonly IReadOnlyList<StatDescriptor> s_descriptors = BuildDescriptors();
        public static IReadOnlyList<StatDescriptor> GetStatDescriptors() => s_descriptors;

        /// <summary>
        /// Build a dictionary mapping property name -> value, for the provided ItemStats.
        /// Keys are the names of the properties from descriptors.
        /// </summary>
        public static IReadOnlyDictionary<string, object> ToDictionary(ItemStats stats)
        {
            ArgumentNullException.ThrowIfNull(stats);

            var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var d in s_descriptors)
            {
                var val = d.Getter(stats);
                dict[d.PropertyName] = ConvertToType(d.PropertyType, val);
            }

            return dict;
        }

        public static object ConvertToType(Type propertyType, object value)
        {
            return Type.GetTypeCode(propertyType) switch
            {
                TypeCode.Double => Convert.ChangeType(value, typeof(double)) ?? 0.0d,
                TypeCode.Int32 => Convert.ChangeType(value, typeof(int)) ?? 0,
                TypeCode.Int64 => Convert.ChangeType(value, typeof(long)) ?? 0,
                TypeCode.String => Convert.ChangeType(value, typeof(string)) ?? string.Empty,
                _ => value,
            };
        }

        private static IReadOnlyList<StatDescriptor> BuildDescriptors()
        {
            var list = new List<StatDescriptor>();
            var props = typeof(ItemStats).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            foreach (var p in props)
            {
                var t = p.PropertyType;
                if (t != typeof(int) && t != typeof(double) && t != typeof(string) && t != typeof(long)) continue; // include numeric and string properties (UI may want Enchant too)

                // Description attribute or fallback
                var descAttr = p.GetCustomAttribute<DescriptionAttribute>(inherit: false);
                var header = descAttr.Description ?? Constants.DEFAULT_DESCRIPTION;

                // optional StatColumnAttribute for ordering
                var orderAttr = p.GetCustomAttribute<StatColumnAttribute>(inherit: false);
                int order = orderAttr?.Order ?? int.MaxValue;

                // compile a fast getter: Func<ItemStats, object>
                var param = Expression.Parameter(typeof(ItemStats), "s");
                var propAccess = Expression.Property(param, p);
                var convert = Expression.Convert(propAccess, typeof(object));
                var lambda = Expression.Lambda<Func<ItemStats, object>>(convert, param).Compile();

                list.Add(new StatDescriptor
                {
                    Property = p,
                    PropertyName = p.Name,
                    Header = header,
                    Order = order,
                    Getter = lambda,
                    PropertyType = t
                });
            }

            // Order by explicit order first (StatColumnAttribute), then by MetadataToken to keep
            // declaration-like ordering when available, then by property name as last resort.
            return [.. list
                .OrderBy(d => d.Order)
                .ThenBy(d => d.Property.MetadataToken)
                .ThenBy(d => d.PropertyName, StringComparer.OrdinalIgnoreCase)];
        }
    }
}
