using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Reflection;
using ConfigEditorComponent.Model;
using ConfigEditorComponent.Helpers;


namespace ConfigEditorComponent.Components.Dictionary
{
	public partial class DictionaryKeyEditor
	{

		[Parameter]
		public PropertyInfo? RootProperty { get; set; }

		[Parameter]
		public object? RootPropertyValue { get; set; }

		[Parameter]
		public string PropertyName { get; set; }
		[Parameter]
		public DictionaryEntry KeyValues { get; set; }

		[Parameter]
		public Func<string, bool> KeyAlreadyExistsMethod { get; set; }

		[Parameter]
		public EventCallback<object> RootLevelOptionChanged { get; set; }

		[Parameter]
		public EventCallback<object> NestedOptionChanged { get; set; }

		private bool IsShowEditor;


		public string? ValidationMessage;

		public DictionaryFormAddRemoveItem DictionaryRemoveField;

		public DictionaryFormKeyEdit DictionaryKeyEditField;

		private void ToggleEditor()
		{
			IsShowEditor = !IsShowEditor;
			//StateHasChanged();
		}
		protected override void OnInitialized()
		{
			DictionaryRemoveField = new DictionaryFormAddRemoveItem();
			DictionaryKeyEditField = new DictionaryFormKeyEdit();

		}

		protected override void OnParametersSet()
		{
			DictionaryRemoveField.PropertyName = PropertyName;
			DictionaryRemoveField.KeyName = KeyValues.Key.ToString();

			DictionaryKeyEditField.currentKeyName = KeyValues.Key.ToString();
			DictionaryKeyEditField.newKeyName = null;
			DictionaryKeyEditField.PropertyName = PropertyName;

			ValidationMessage = null;
		}
		private async Task RemoveItem()
		{
			PropertyInfo propertyInfo;
			Type propertyType;
			object propertyValue;
			if (RootProperty == null)
			{
				propertyInfo = RootPropertyValue.GetType().GetProperty(DictionaryRemoveField.PropertyName);
				propertyType = RootPropertyValue.GetType().GetProperty(DictionaryRemoveField.PropertyName).PropertyType;
			}
			else
			{
				propertyInfo = RootProperty.PropertyType.GetProperty(DictionaryRemoveField.PropertyName);
				propertyType = RootProperty.PropertyType.GetProperty(DictionaryRemoveField.PropertyName).PropertyType;

			}

			propertyValue = propertyInfo.GetValue(RootPropertyValue);



			if (ConfigEditorHelper.isDictionaryType(propertyInfo))
			{

				MethodInfo removeMethod = propertyType.GetMethod("Remove", new Type[] { typeof(string) });

				removeMethod.Invoke(propertyValue, new object[] { DictionaryRemoveField.KeyName });

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


		}

		private async Task EditDictionaryKey()
		{
			if (DictionaryKeyEditField.currentKeyName == DictionaryKeyEditField.newKeyName)
			{
				ValidationMessage = "current value and new entered value are same";
			}

			else if (KeyAlreadyExistsMethod.Invoke(DictionaryKeyEditField.newKeyName))
			{
				ValidationMessage = "key already exists";
			}
			else
			{

				PropertyInfo propertyInfo;
				Type propertyType;
				object propertyValue;
				if (RootProperty == null)
				{
					propertyInfo = RootPropertyValue.GetType().GetProperty(DictionaryKeyEditField.PropertyName);
					propertyType = RootPropertyValue.GetType().GetProperty(DictionaryKeyEditField.PropertyName).PropertyType;
					propertyValue = propertyInfo.GetValue(RootPropertyValue);
				}
				else
				{
					propertyInfo = RootProperty.PropertyType.GetProperty(DictionaryKeyEditField.PropertyName);
					propertyType = RootProperty.PropertyType.GetProperty(DictionaryKeyEditField.PropertyName).PropertyType;
					propertyValue = propertyInfo.GetValue(RootPropertyValue);
				}


				if (ConfigEditorHelper.isDictionaryType(propertyInfo))
				{
					var dictionaryType = propertyValue.GetType();

					// Get the Dictionary's Add method
					MethodInfo addMethod = propertyType.GetMethod("Add");

					// Get the dictionary's indexer property
					var indexerProperty = dictionaryType.GetProperty("Item");

					// Get the value associated with the old key
					var value = indexerProperty.GetValue(propertyValue, new object[] { DictionaryKeyEditField.currentKeyName });

					// Remove the old key
					MethodInfo removeMethod = dictionaryType.GetMethod("Remove", new Type[] { typeof(string) });
					removeMethod.Invoke(propertyValue, new object[] { DictionaryKeyEditField.currentKeyName });

					// Add the value with the new key
					addMethod.Invoke(propertyValue, new object[] { DictionaryKeyEditField.newKeyName, value });

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

				IsShowEditor = false;
			}

		}
	}
}
