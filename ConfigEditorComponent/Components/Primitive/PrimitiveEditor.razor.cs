using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using ConfigEditorComponent.Model;

namespace ConfigEditorComponent.Components.Primitive
{
	public partial class PrimitiveEditor : ComponentBase
	{

		[Parameter]
		public PropertyInfo? RootProperty { get; set; }

		[Parameter]
		public object? RootPropertyValue { get; set; }
		[Parameter]
		public PropertyInfo? Property { get; set; }

		[Parameter]
		public string PropertyName { get; set; }

		[Parameter]
		public object PropertyValue { get; set; }

		[Parameter]
		public EventCallback<object> RootLevelOptionChanged { get; set; }

		[Parameter]
		public EventCallback<object> NestedOptionChanged { get; set; }


		private bool IsShowEditor;

		private string? ValidationMessage;

		private PrimitiveItem? primitiveItem;

		private bool isNullable;
		private TypeCode code;
		private void ToggleEditor()
		{
			IsShowEditor = !IsShowEditor;
			ValidationMessage = null;
		}
		private void ToggleInputField(ChangeEventArgs e)
		{
			primitiveItem.isDisabled = (bool)e.Value;
		}

        protected override void OnParametersSet()
        {
            primitiveItem = new PrimitiveItem();
            this.AssignValuesToObject();
        }

		private void AssignValuesToObject()
		{

			var propertyType = Nullable.GetUnderlyingType(Property.PropertyType) ?? Property.PropertyType;
			isNullable = Nullable.GetUnderlyingType(Property.PropertyType) != null;
			if (PropertyValue == null)
			{
				primitiveItem.isNull = true;
				primitiveItem.isDisabled = true;

			}

			primitiveItem.PropertyName = PropertyName;
			primitiveItem.Type = propertyType;


			code = Type.GetTypeCode(propertyType);


			switch (code)
			{
				case TypeCode.String:
					primitiveItem.StringKindValue = Convert.ToString(PropertyValue);
					break;
				case TypeCode.Int32:
					primitiveItem.IntKindValue = Convert.ToInt32(PropertyValue);
					break;

				case TypeCode.Int64:
					primitiveItem.LongKindValue = Convert.ToInt64(PropertyValue);
					break;

				case TypeCode.Double:
					primitiveItem.DoubleKindValue = Convert.ToDouble(PropertyValue);
					break;

				case TypeCode.Boolean:
					primitiveItem.BooleanKindValue = Convert.ToBoolean(PropertyValue);
					break;
				default:
					Console.WriteLine($"PrimitiveEditor - Data type is not supported for Property : {PropertyName} with type code : {code}");
					break;
			}


		}

		private async Task SaveItemAsync()
		{
			try
			{
				ValidationMessage = null;
				this.EditItem();
				if (RootProperty == null)
				{
					await RootLevelOptionChanged.InvokeAsync(RootPropertyValue);

				}
				else
				{
					await NestedOptionChanged.InvokeAsync(RootPropertyValue);
				}


				primitiveItem = null;
				IsShowEditor = false;
			}
			catch (ArgumentException ex)
			{
				ValidationMessage = ex.Message;

			}
		}

		private void EditItem()
		{

			PropertyInfo property;

			if (RootProperty == null)
			{

				property = RootPropertyValue.GetType().GetProperty(primitiveItem.PropertyName);
			}
			else
			{
				property = RootProperty.PropertyType.GetProperty(primitiveItem.PropertyName);
			}

			//Retrieve all attributes applied to the property
			var attributes = property.GetCustomAttributes().OfType<ValidationAttribute>();

			if (property != null && property.CanWrite && RootPropertyValue != null)
			{
				switch (code)
				{
					case TypeCode.String:
						foreach (var attr in attributes)
						{
							if (!attr.IsValid(primitiveItem.StringKindValue))
							{
								var msg = attr.ErrorMessage ?? attr.FormatErrorMessage(property.Name);
								throw new ArgumentException(msg);
							}
						}
						primitiveItem.StringKindValue = primitiveItem.isDisabled ? null : primitiveItem.StringKindValue;
						property.SetValue(RootPropertyValue, primitiveItem.StringKindValue);
						break;

					case TypeCode.Int32:
						foreach (var attr in attributes)
						{
							if (!attr.IsValid(primitiveItem.IntKindValue))
							{
								var msg = attr.ErrorMessage ?? attr.FormatErrorMessage(property.Name);
								throw new ArgumentException(msg);
							}
						}
						primitiveItem.IntKindValue = (primitiveItem.isDisabled && isNullable) ? null : primitiveItem.IntKindValue;
						property.SetValue(RootPropertyValue, primitiveItem.IntKindValue);
						break;

					case TypeCode.Int64:
						
						foreach (var attr in attributes)
						{
							if (!attr.IsValid(primitiveItem.LongKindValue))
							{
								var msg = attr.ErrorMessage ?? attr.FormatErrorMessage(property.Name);
								throw new ArgumentException(msg);
							}
						}
						primitiveItem.LongKindValue = (primitiveItem.isDisabled && isNullable) ? null : primitiveItem.LongKindValue;
						property.SetValue(RootPropertyValue, primitiveItem.LongKindValue);
						break;

					case TypeCode.Double:
						foreach (var attr in attributes)
						{
							if (!attr.IsValid(primitiveItem.DoubleKindValue))
							{
								var msg = attr.ErrorMessage ?? attr.FormatErrorMessage(property.Name);
								throw new ArgumentException(msg);
							}
						}
						primitiveItem.DoubleKindValue = (primitiveItem.isDisabled && isNullable) ? null : primitiveItem.DoubleKindValue;
						property.SetValue(RootPropertyValue, primitiveItem.DoubleKindValue);
						break;

					case TypeCode.Boolean:
						primitiveItem.BooleanKindValue = (primitiveItem.isDisabled && isNullable) ? null : primitiveItem.BooleanKindValue;
						property.SetValue(RootPropertyValue, primitiveItem.BooleanKindValue);
						break;

				}

			}



		}


	}

}
