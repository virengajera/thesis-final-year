using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ConfigEditorComponent.Model;
using ConfigEditorComponent.Helpers;

namespace ConfigEditorComponent.Components.Dictionary
{
    public partial class DictionaryValueEditor
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
        public object? PropertyValue { get; set; }

        [Parameter]
        public string KeyName { get; set; }

        [Parameter]
        public string RootPropertyName { get; set; }

        [Parameter]
        public EventCallback<object> RootLevelOptionChanged { get; set; }

        [Parameter]
        public EventCallback<object> NestedOptionChanged { get; set; }

        private bool IsShowEditor;

        private string? ValidationMessage;

        private DictionaryFormValueEdit DictionaryValueEditField;

        private bool isNullable;
        private TypeCode code;
        private void ToggleEditor()
        {
            IsShowEditor = !IsShowEditor;
            //StateHasChanged();
        }

        private void ToggleInputField(ChangeEventArgs e)
        {
            DictionaryValueEditField.isDisabled = (bool)e.Value;
        }

        protected override void OnParametersSet()
        {
            DictionaryValueEditField = new DictionaryFormValueEdit();

            ValidationMessage = null;

            this.AssignValueToObject();
        }
        private void AssignValueToObject()
        {
            DictionaryValueEditField.RootPropertyName = RootPropertyName;
            DictionaryValueEditField.PropertyName = PropertyName;
            DictionaryValueEditField.KeyName = KeyName;


            var propertyType = Nullable.GetUnderlyingType(Property.PropertyType) ?? Property.PropertyType;
            isNullable = Nullable.GetUnderlyingType(Property.PropertyType) != null;

            if (PropertyValue == null)
            {
                DictionaryValueEditField.isNull = true;
                DictionaryValueEditField.isDisabled = true;

            }

            DictionaryValueEditField.Type = propertyType;

            code = Type.GetTypeCode(propertyType);



            switch (code)
            {

                case TypeCode.String:
                    DictionaryValueEditField.StringKindValue = PropertyValue != null ? (string)PropertyValue : null;
                    break;

                case TypeCode.Int32:
                    DictionaryValueEditField.IntKindValue = PropertyValue != null ? (int)PropertyValue : null;
                    break;

                case TypeCode.Int64:
                    DictionaryValueEditField.LongKindValue = PropertyValue != null ? (long)PropertyValue : null;
                    break;

                case TypeCode.Double:
                    DictionaryValueEditField.DoubleKindValue = PropertyValue != null ? (double)PropertyValue : null;
                    break;

                case TypeCode.Boolean:
                    //DictionaryValueEditField.BooleanKindValue = PropertyValue != null ? (bool)PropertyValue : null;
                    DictionaryValueEditField.BooleanKindValue = Convert.ToBoolean(PropertyValue);
                    break;

                default:
                    Console.WriteLine($"DictionaryValueEditor - Data type is not supported for Property : {PropertyName} with type code : {code}");
                    break;
            }

        }

        private async Task EditDictionaryValue()
        {
            try
            {
                PropertyInfo propertyInfo;
                Type propertyType;

                // propertyInfo.PropertyType.GetGenericArguments()

                if (RootProperty == null)
                {
                    propertyInfo = RootPropertyValue.GetType().GetProperty(DictionaryValueEditField.RootPropertyName);
                    propertyType = RootPropertyValue.GetType().GetProperty(DictionaryValueEditField.RootPropertyName).PropertyType;

                }
                else
                {
                    propertyInfo = RootProperty.PropertyType.GetProperty(DictionaryValueEditField.RootPropertyName);
                    propertyType = RootProperty.PropertyType.GetProperty(DictionaryValueEditField.RootPropertyName).PropertyType;
                }

                object propertyValue = propertyInfo.GetValue(RootPropertyValue);

                if (ConfigEditorHelper.isDictionaryType(propertyInfo))
                {
                    var dictionaryType = propertyValue.GetType();

                    // Get the Dictionary's Add method
                    MethodInfo addMethod = propertyType.GetMethod("Add");

                    // Get the dictionary's indexer property
                    var indexerProperty = dictionaryType.GetProperty("Item");

                    // Get the value associated with the old key
                    var value = indexerProperty.GetValue(propertyValue, new object[] { DictionaryValueEditField.KeyName });


                    // Get the type of the value within the dictionary
                    Type valueType = value.GetType();
                    if (valueType != null)
                    {
                        // Get the property to update
                        PropertyInfo propertyToUpdate = valueType.GetProperty(DictionaryValueEditField.PropertyName);
                        // Retrieve all attributes applied to the property


                        if (propertyToUpdate != null)
                        {
                        var attributes = propertyToUpdate.GetCustomAttributes().OfType<ValidationAttribute>();
                            TypeCode code = Type.GetTypeCode(DictionaryValueEditField.Type);

                            switch (code)
                            {
                                case TypeCode.String:
                                    foreach (var attr in attributes)
                                    {
                                        if (!attr.IsValid(DictionaryValueEditField.StringKindValue))
                                        {
                                            var msg = attr.ErrorMessage ?? attr.FormatErrorMessage(propertyToUpdate.Name);
                                            throw new ArgumentException(msg);
                                        }
                                    }
                                    DictionaryValueEditField.StringKindValue = DictionaryValueEditField.isDisabled ? null : DictionaryValueEditField.StringKindValue;
                                    propertyToUpdate.SetValue(value, DictionaryValueEditField.StringKindValue);
                                    break;

                                case TypeCode.Int32:
                                    foreach (var attr in attributes)
                                    {
                                        if (!attr.IsValid(DictionaryValueEditField.IntKindValue))
                                        {
                                            var msg = attr.ErrorMessage ?? attr.FormatErrorMessage(propertyToUpdate.Name);
                                            throw new ArgumentException(msg);
                                        }
                                    }
                                    DictionaryValueEditField.IntKindValue = (DictionaryValueEditField.isDisabled && isNullable) ? null : DictionaryValueEditField.IntKindValue;
                                    propertyToUpdate.SetValue(value, DictionaryValueEditField.IntKindValue);
                                    break;

                                case TypeCode.Int64:
                                    foreach (var attr in attributes)
                                    {
                                        if (!attr.IsValid(DictionaryValueEditField.LongKindValue))
                                        {
                                            var msg = attr.ErrorMessage ?? attr.FormatErrorMessage(propertyToUpdate.Name);
                                            throw new ArgumentException(msg);
                                        }
                                    }
                                    DictionaryValueEditField.LongKindValue = (DictionaryValueEditField.isDisabled && isNullable) ? null : DictionaryValueEditField.LongKindValue;
                                    propertyToUpdate.SetValue(value, DictionaryValueEditField.LongKindValue);
                                    break;

                                case TypeCode.Double:
                                    foreach (var attr in attributes)
                                    {
                                        if (!attr.IsValid(DictionaryValueEditField.DoubleKindValue))
                                        {
                                            var msg = attr.ErrorMessage ?? attr.FormatErrorMessage(propertyToUpdate.Name);
                                            throw new ArgumentException(msg);
                                        }
                                    }
                                    DictionaryValueEditField.DoubleKindValue = (DictionaryValueEditField.isDisabled && isNullable) ? null : DictionaryValueEditField.DoubleKindValue;
                                    propertyToUpdate.SetValue(value, DictionaryValueEditField.DoubleKindValue);
                                    break;

                                case TypeCode.Boolean:
                                    DictionaryValueEditField.BooleanKindValue = (DictionaryValueEditField.isDisabled && isNullable) ? null : DictionaryValueEditField.BooleanKindValue;
                                    propertyToUpdate.SetValue(value, DictionaryValueEditField.BooleanKindValue);
                                    break;

                            }

                            indexerProperty.SetValue(propertyValue, value, new object[] { DictionaryValueEditField.KeyName });

                        }

                        else
                        {

                            Type keyType = propertyType.GetGenericArguments()[0];
                            Type valueType2 = propertyType.GetGenericArguments()[1];
                            object? newValue = null;

                            switch (Type.GetTypeCode(valueType2))
                            {

                                case TypeCode.String:
                                    newValue = DictionaryValueEditField.StringKindValue;
                                    break;

                                case TypeCode.Int32:
                                    newValue = DictionaryValueEditField.IntKindValue;
                                    break;

                                case TypeCode.Int64:
                                    newValue = DictionaryValueEditField.LongKindValue;
                                    break;

                                case TypeCode.Double:
                                    newValue = DictionaryValueEditField.DoubleKindValue;
                                    break;

                                case TypeCode.Boolean:
                                    //DictionaryValueEditField.BooleanKindValue = PropertyValue != null ? (bool)PropertyValue : null;
                                    newValue = DictionaryValueEditField.BooleanKindValue;
                                    break;

                                default:
                                    Console.WriteLine($"DictionaryValueEditor - Data type is not supported for Property : {PropertyName} with type code : {code}");
                                    break;
                            }





                            // Convert the key name to the key type
                            object convertedKey = Convert.ChangeType(DictionaryValueEditField.KeyName, keyType);
                            object convertedValue = Convert.ChangeType(newValue, valueType2);
                            // Use reflection to call the dictionary's indexer to set the new value
                            var setMethod = propertyType.GetProperty("Item").SetMethod;
                            setMethod.Invoke(propertyValue, new object[] { convertedKey, convertedValue });
                        }
                    }
                  


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
                ValidationMessage = null;
            }
            catch (ArgumentException ex)
            {
                ValidationMessage = ex.Message;
            }


        }
    }
}
