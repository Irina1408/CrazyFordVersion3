using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CrazyFord
{
    public static class ViewHelper
    {
        public static HashSet<KeyValuePair<PropertyInfo, Binding>> GetBindingProperties<T>(T t)
        {
            var result = new HashSet<KeyValuePair<PropertyInfo, Binding>>();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                if(!property.CanRead || !property.CanWrite) continue;

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
                var rd = new RowDefinition(){Height = GridLength.Auto};
                result.RowDefinitions.Add(rd);

                if(propertyValue.Key.PropertyType == typeof(bool))
                {
                    var chb = new CheckBox {Content = propertyValue.Key.GetPropertyDescription()};
                    chb.SetBinding(CheckBox.IsCheckedProperty, propertyValue.Value);
                    Grid.SetColumn(chb, 0);
                    Grid.SetColumnSpan(chb, 2);
                    Grid.SetRow(chb, result.RowDefinitions.IndexOf(rd));

                    // add to the grid
                    result.Children.Add(chb);
                }
                else
                {
                    var tb = new TextBox();
                    tb.SetBinding(TextBox.TextProperty, propertyValue.Value);
                    // description
                    var lbl = new Label() { Content = propertyValue.Key.GetPropertyDescription() };
                    Grid.SetColumn(tb, 1);
                    Grid.SetColumn(lbl, 0);
                    Grid.SetRow(tb, result.RowDefinitions.IndexOf(rd));
                    Grid.SetRow(lbl, result.RowDefinitions.IndexOf(rd));

                    // add to the grid
                    result.Children.Add(tb);
                    result.Children.Add(lbl);
                }
            }

            // configure grid
            result.Margin = new Thickness(10);

            return result;
        }

        public static string GetPropertyDescription(this PropertyInfo propertyInfo)
        {
            foreach (var attribute in propertyInfo.CustomAttributes.Where(attribute => attribute.AttributeType == typeof (DescriptionAttribute)))
            {
                return attribute.ConstructorArguments[0].Value.ToString();
            }

            return propertyInfo.Name;
        }
    }
}
