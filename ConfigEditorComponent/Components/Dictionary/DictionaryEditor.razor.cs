using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Reflection;
using ConfigEditorComponent.Model;
using ConfigEditorComponent.Helpers;

namespace ConfigEditorComponent.Components.Dictionary
{
	public partial class DictionaryEditor 
	{
		[Parameter]
		public PropertyInfo? RootProperty { get; set; }

		[Parameter]
		public object? RootPropertyValue { get; set; }

		[Parameter]
		public PropertyInfo Property { get; set; }

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

		public DictionaryFormAddRemoveItem DictionaryFormField = new DictionaryFormAddRemoveItem();

		private void ToggleEditor()
		{
			IsShowEditor = !IsShowEditor;
		}

		

		protected override void OnParametersSet()
		{

			DictionaryFormField.KeyName = "";
			DictionaryFormField.PropertyName = PropertyName;
			IsShowEditor = false;

		}

		private async Task AddDictionaryItem()
		{
			if (this.KeyAlreadyExists(DictionaryFormField.KeyName))
			{
				ValidationMessage = "Key Already exists.";
			}
			else
			{
				PropertyInfo propertyInfo;
				Type propertyType;
				
				if (RootProperty == null)
				{
					propertyInfo = RootPropertyValue.GetType().GetProperty(DictionaryFormField.PropertyName);
					propertyType = RootPropertyValue.GetType().GetProperty(DictionaryFormField.PropertyName).PropertyType;
				}
				else
				{
					propertyInfo = RootProperty.PropertyType.GetProperty(DictionaryFormField.PropertyName);
					propertyType = RootProperty.PropertyType.GetProperty(DictionaryFormField.PropertyName).PropertyType;
					
				}

				object propertyValue = propertyInfo.GetValue(RootPropertyValue);

				if (ConfigEditorHelper.isDictionaryType(propertyInfo))
				{
					Type customClassType = propertyType.GetGenericArguments()[1];

					object value;
					if(customClassType == typeof(string))
					{
						value = "";

                    }
					else
					{
						value = Activator.CreateInstance(customClassType);

                    }

					MethodInfo addMethod = propertyType.GetMethod("Add");

					addMethod.Invoke(propertyValue, new object[] { DictionaryFormField.KeyName, value });

					propertyInfo.SetValue(RootPropertyValue, propertyValue);

				}

				if (RootProperty == null)
				{
					await RootLevelOptionChanged.InvokeAsync(RootPropertyValue);

				}
				else
				{
					await NestedOptionChanged.InvokeAsync(RootPropertyValue);
				}

				ValidationMessage = null;
				IsShowEditor = false;

			}




		}

		public bool KeyAlreadyExists(string keyName)
		{
			foreach (DictionaryEntry dictItem in (IDictionary)PropertyValue)
			{
				if (keyName == dictItem.Key.ToString())
				{
					return true;
				}
			}
			return false;
		}
	}
}
