using ConfigEditorComponent.Component;
using System.ComponentModel;
using System.Reflection;

namespace ConfigEditorComponent.Helpers
{
	internal static class ConfigEditorHelper
	{
		public static bool isCollapsableAttrDefined(PropertyInfo p)
		{
			if (p == null) return false;
			return Attribute.IsDefined(p, typeof(ConfigEditorCollapsable), true);
		}

		public static int getSpacingValueOfCollapsableAttr(PropertyInfo p)
		{
			if (p == null) return 0;
			var collapsableAttr = (ConfigEditorCollapsable)Attribute.GetCustomAttribute(p, typeof(ConfigEditorCollapsable), true);
			if (collapsableAttr == null) return 0;
			return collapsableAttr.spacingTopBottom;
		}

		public static bool isHiddenAttrDefined(PropertyInfo p)
		{
			if (p == null) return false;
			return Attribute.IsDefined(p, typeof(ConfigEditorHidden), true);
		}

		public static bool isDescriptionAttrDefined(PropertyInfo p)
		{
			if (p == null) return false;
			return Attribute.IsDefined(p, typeof(DescriptionAttribute));
		}

		public static string? getDescriptionAttrValue(PropertyInfo p)
		{
			DescriptionAttribute descriptionAttr = (DescriptionAttribute)Attribute.GetCustomAttribute(p, typeof(DescriptionAttribute));
			return descriptionAttr.Description;
		}

		public static IEnumerable<PropertyInfo> GetProperties(Type value)
		{
			return value.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanWrite && !isHiddenAttrDefined(p));
		}

		public static bool isPrimitiveType(PropertyInfo p)
		{
			var propertyType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
			TypeCode code = Type.GetTypeCode(propertyType);

			if (code == TypeCode.String || code == TypeCode.Int32 || code == TypeCode.Int64 || code == TypeCode.Boolean)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool isDictionaryType(PropertyInfo p)
		{
			var propertyType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
			TypeCode code = Type.GetTypeCode(propertyType);
			return propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>);
		}

		public static bool isListType(PropertyInfo p)
		{
			var propertyType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
			TypeCode code = Type.GetTypeCode(propertyType);
			return propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>);
		}

		public static IEnumerable<PropertyInfo> GetPropertiesSortedByName(Type value)
		{
			var temp = GetProperties(value).ToList();

			var result1 = temp.Where(isPrimitiveType).OrderBy(p => p.Name);

			var result2 = temp.Where(p =>
			{
				var propertyType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
				TypeCode code = Type.GetTypeCode(propertyType);

				bool isDictionary = isDictionaryType(p);
				bool isList = isListType(p);

				if (code == TypeCode.Object && (propertyType.IsClass || isDictionary || isList))
				{
					return true;
				}
				else
				{
					return false;
				}
			}).OrderBy(p => p.Name);


			//return result1.Concat(result2);

			return GetProperties(value).ToList();
		}

	}
}
