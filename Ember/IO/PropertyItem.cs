using Newtonsoft.Json;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Ember.IO
{
    public class PropertyItem
    {
        /// <summary>
        /// Name of this Property
        /// </summary>
        [JsonRequired]
        public string Name { get; set; }

        /// <summary>
        /// Tooltip of this Property
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Name of this Property in the config
        /// </summary>
        [JsonRequired]
        public string ConfigName { get; set; }

        /// <summary>
        /// Type of the Value of this Property
        /// </summary>
        [JsonRequired]
        public string ValueType { get; set; }

        /// <summary>
        /// Wheter this Property is recommended
        /// </summary>
        public bool IsRecommended { get; set; }

        public PropertyItem(string name, string tooltip, string configName, bool isRecommended, Type valueType)
        {
            this.Name = name;
            this.Tooltip = tooltip;
            this.ConfigName = configName;
            this.IsRecommended = isRecommended;
            this.ValueType = valueType.ToString();
        }

        /// <summary>
        /// Returns a WPF Element of this type
        /// </summary>
        public UIElement GetElementFromType(object defaultValue)
        {
            Type propertyType = GetType();

            if (propertyType == typeof(uint) || propertyType == typeof(float) || propertyType == typeof(string))
            {
                StackPanel property = new StackPanel()
                {
                    Name = this.ConfigName,
                    Orientation = Orientation.Horizontal
                };
                property.Children.Add(new TextBlock()
                {
                    Margin = new Thickness(20, 0, 10, 0),
                    Text = this.Name
                });
                property.Children.Add(new TextBox()
                {
                    TextWrapping = TextWrapping.Wrap,
                    VerticalAlignment = VerticalAlignment.Top,
                    MinWidth = 100,
                    ToolTip = this.Tooltip,
                    Text = defaultValue.ToString()
                });

                return property;
            }
            else if (propertyType == typeof(bool))
            {
                return new CheckBox()
                {
                    Name = this.ConfigName,
                    Content = this.Name,
                    ToolTip = this.Tooltip,
                    IsChecked = (bool)defaultValue
                };
            }

            return null;
        }

        /// <summary>
        /// Gets the Type of this Property based on it's <see cref="ValueType"/>
        /// </summary>
        public new Type GetType()
        {
            return Type.GetType(this.ValueType);
        }
    }
}
