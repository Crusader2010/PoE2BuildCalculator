using System.Collections.Immutable;
using System.Reflection;

using Domain.Enums;
using Domain.Helpers;
using Domain.Validation;

namespace Domain.Serialization
{
	public static class SerializationExtensions
	{
		private static readonly IReadOnlyList<ItemStatsHelper.StatDescriptor> _statDescriptors
			= ItemStatsHelper.GetStatDescriptors();

		private static readonly ImmutableDictionary<string, PropertyInfo> _propLookup
			= _statDescriptors.ToImmutableDictionary(d => d.PropertyName, d => d.Property, StringComparer.OrdinalIgnoreCase);

		// Group → GroupDto
		public static GroupDto ToDto(this Group group) => new()
		{
			GroupId = group.GroupId,
			GroupName = group.GroupName,
			Stats = group.Stats?.Select(s => new GroupStatDto
			{
				PropertyName = s.PropertyName,
				Operator = s.Operator?.ToString()
			}).ToList() ?? []
		};

		// GroupDto → Group
		public static Group ToGroup(this GroupDto dto) => new()
		{
			GroupId = dto.GroupId,
			GroupName = dto.GroupName,
			Stats = dto.Stats?.Select(s => new GroupStatModel
			{
				PropertyName = s.PropertyName,
				PropInfo = _propLookup.GetValueOrDefault(s.PropertyName),
				Operator = string.IsNullOrEmpty(s.Operator)
					? null
					: Enum.Parse<ArithmeticOperationsEnum>(s.Operator)
			}).ToList() ?? []
		};
	}
}
