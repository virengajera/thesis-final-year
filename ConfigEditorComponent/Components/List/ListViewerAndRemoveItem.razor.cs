using ConfigEditorComponent.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditorComponent.Components.List
{
    public partial class ListViewerAndRemoveItem : ComponentBase
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
        public List<object> PropertyValue { get; set; }

        [Parameter]
        public EventCallback<object> RootLevelOptionChanged { get; set; }

        [Parameter]
        public EventCallback<object> NestedOptionChanged { get; set; }

        private bool isNullable;

        private async Task RemoveItemFromList(int index)
        {
            PropertyValue?.RemoveAt(index);
            await ListChanged(PropertyValue);
        }

        private async Task ListChanged(List<object> newList)
        {
            PropertyInfo? property;
            if (RootProperty == null)
            {
                property = RootPropertyValue?.GetType().GetProperty(PropertyName);
            }
            else
            {
                property = RootProperty.PropertyType.GetProperty(PropertyName);
            }

            if (property != null && property.CanWrite && RootPropertyValue != null)
            {
                Type propertyType = property.PropertyType;
                // Check if it's a generic type and get the type of the items
                if (propertyType.IsGenericType)
                {
                    Type itemType = propertyType.GenericTypeArguments[0];
                    // Create an instance of the list type
                    var typedList = (IList)Activator.CreateInstance(propertyType);

                    foreach (var item in newList)
                    {
                        // Cast the item to the correct type
                        var typedItem = Convert.ChangeType(item, itemType);
                        typedList.Add(typedItem);
                    }

                    property.SetValue(RootPropertyValue, typedList);
                }
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

    }

}
