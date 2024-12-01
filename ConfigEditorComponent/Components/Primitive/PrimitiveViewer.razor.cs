using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace ConfigEditorComponent.Components.Primitive
{
	public partial class PrimitiveViewer : ComponentBase
	{
		[Parameter]
		public PropertyInfo? Property {  get; set; }
		[Parameter]
		public string PropertyName { get; set; }

		[Parameter]
		public object PropertyValue { get; set; }

		private TypeCode code;
		private bool isNullable;

		protected override void OnParametersSet()
		{
			var propertyType = Nullable.GetUnderlyingType(Property.PropertyType) ?? Property.PropertyType;
			isNullable = Nullable.GetUnderlyingType(Property.PropertyType) != null;
			code = Type.GetTypeCode(propertyType);
		}
	}
}
