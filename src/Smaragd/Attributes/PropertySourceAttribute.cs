using System;
using System.Collections.Generic;

namespace NKristek.Smaragd.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// <para>
    /// This <see cref="Attribute"/> can be used on properties in a class inheriting from <see cref="T:NKristek.Smaragd.ViewModels.ComputedBindableBase" />.
    /// It indicates, that the property depends on one or multiple properties.
    /// </para>
    /// <para>
    /// <see cref="E:System.ComponentModel.INotifyPropertyChanged.PropertyChanged" /> will be raised, when <see cref="E:System.ComponentModel.INotifyPropertyChanged.PropertyChanged" /> is raised for one of the given property names.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertySourceAttribute
        : Attribute
    {
        /// <summary>
        /// Property names which should raise <see cref="E:System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/>.
        /// </summary>
        public IEnumerable<string> PropertySources { get; set; }

        /// <summary>
        /// Indicates if the attributes from the base class should be considered.
        /// </summary>
        public bool InheritAttributes { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:NKristek.Smaragd.Attributes.PropertySourceAttribute" /> class with one or multiple names of properties the property depends on.
        /// </summary>
        /// <param name="propertyNames">Names of source properties.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="propertyNames"/> is null.</exception>
        public PropertySourceAttribute(params string[] propertyNames)
        {
            PropertySources = propertyNames ?? throw new ArgumentNullException(nameof(propertyNames));
        }
    }
}