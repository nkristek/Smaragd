using System;
using System.Collections.Generic;
using System.ComponentModel;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Attributes
{
    /// <summary>
    /// This attribute can be used on properties in classes inheriting from <see cref="ComputedBindableBase"/>.
    /// <para />
    /// It indicates, that the property depends on one or multiple properties.
    /// Given one or multiple property names, an additional <see cref="INotifyPropertyChanged.PropertyChanged"/> event will be raised for this property, if one was raised for one of the specified property names.
    /// <para />
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertySourceAttribute
        : Attribute
    {
        public IEnumerable<string> PropertySources { get; }
        
        public PropertySourceAttribute(params string[] propertyNames)
        {
            PropertySources = propertyNames;
        }
    }
}
