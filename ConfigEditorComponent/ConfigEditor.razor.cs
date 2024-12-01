using ConfigEditorComponent.Model;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditorComponent
{
	public partial class ConfigEditor : ComponentBase
	{
		[Parameter]
		public object OptionValue { get; set; }

		[Parameter]
		public EventCallback<object> OptionValueChanged { get; set; }

		private object _settings;
		protected override void OnInitialized()
		{
			if (OptionValue != null)
			{

				_settings = OptionValue;
			}


		}

		protected override void OnParametersSet()
		{
			if (OptionValue != null)
			{
				_settings = OptionValue;

			}

		}
		private async void RootLevelOptionChanged(object newSettingsOptions)
		{
			_settings = newSettingsOptions;
			await OptionValueChanged.InvokeAsync(_settings);
			StateHasChanged();
		}

		private async void NestedClassOptionChanged(NestedOptionChange option)
		{
			_settings.GetType().GetProperty(option.PropertyName).SetValue(_settings, option.PropertyValue);
			await OptionValueChanged.InvokeAsync(_settings);
			StateHasChanged();

		}

		private void ListValueChanged(object value)
		{
			Console.WriteLine();
		}


    }
}
