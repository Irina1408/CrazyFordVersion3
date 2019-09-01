using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace CrazyFord.View
{
    public static class ViewHelper
    {
        public static HashSet<KeyValuePair<PropertyInfo, Binding>> GetBindingProperties<T>(T t)
        {
            var result = new HashSet<KeyValuePair<PropertyInfo, Binding>>();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var bind = new Binding(property.Name)
                {
                    Source = t, 
                    Mode = BindingMode.TwoWay
                };

                result.Add(new KeyValuePair<PropertyInfo, Binding>(property, bind));
            }

            return result;
        }

        public static Grid GenerateGrid<T>(T t)
        {
            return GenerateGrid(GetBindingProperties<T>(t));
        }

        public static Grid GenerateGrid(HashSet<KeyValuePair<PropertyInfo, Binding>> hashSet)
        {
            var result = new Grid();

            // column definition for description
            result.ColumnDefinitions.Add(new ColumnDefinition());
            // column definition for value
            result.ColumnDefinitions.Add(new ColumnDefinition());

            foreach (var propertyValue in hashSet)
            {
                // definition for new property
                var rd = new RowDefinition();
                result.RowDefinitions.Add(rd);

                if(propertyValue.Key.PropertyType == typeof(bool))
                {
                    var cb = new CheckBox {Content = propertyValue.Key.GetPropertyDescription()};
                    cb.SetBinding(CheckBox.IsCheckedProperty, propertyValue.Value);
                    Grid.SetColumn(cb, 0);
                    Grid.SetColumnSpan(cb, 2);

                    // add to the grid
                    result.Children.Add(cb);
                }
                else
                {
                    var tb = new TextBox();
                    tb.SetBinding(TextBox.TextProperty, propertyValue.Value);
                    // description
                    var lbl = new Label() { Content = propertyValue.Key.GetPropertyDescription() };
                    Grid.SetColumn(tb, 1);
                    Grid.SetColumn(lbl, 0);

                    // add to the grid
                    result.Children.Add(tb);
                    result.Children.Add(lbl);
                }
            }

            return result;
        }

        public static string GetPropertyDescription(this PropertyInfo propertyInfo)
        {
            if (propertyInfo.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(DescriptionAttribute)))
            {
                return propertyInfo.ToString();
            }

            return propertyInfo.Name;
        }
    }
}
