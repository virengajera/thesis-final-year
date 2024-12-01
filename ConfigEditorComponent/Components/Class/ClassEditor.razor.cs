using System.Reflection;
using Microsoft.AspNetCore.Components;
using ConfigEditorComponent.Model;

namespace ConfigEditorComponent.Components.Class
{
	public partial class ClassEditor 
	{
		[Parameter]
		public PropertyInfo? RootProperty { get; set; }

		[Parameter]
		public PropertyInfo Property { get; set; }

		[Parameter]
		public string PropertyName { get; set; }

		[Parameter]
		public object? RootPropertyValue { get; set; }
		[Parameter]
		public object? PropertyValue { get; set; }

		
		[Parameter]
		public EventCallback<NestedOptionChange> NestedClassOptionChanged { get; set; }


		[Parameter]
		public ClassEditor Parent { get;set; }

		public async void NestedValueChanged(object value)
		{
			if (RootProperty != null)
			{
				PropertyInfo property = RootProperty.PropertyType.GetProperty(PropertyName);
				if (property != null && property.CanWrite)
				{
					property.SetValue(RootPropertyValue, value);
				}

				NotifyParentUpdate();

			}
			else
			{
				
				await NestedClassOptionChanged.InvokeAsync(new NestedOptionChange()
				{
					PropertyName = PropertyName,
					PropertyValue = PropertyValue
				});

			}
		}

		private void NotifyParentUpdate()
		{
			if(RootProperty!= null)
			{
				Parent.NestedValueChanged(RootPropertyValue);
			}
		}

	}
}
