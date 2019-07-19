using System;
using System.Collections.Generic;
using System.ComponentModel;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// <para>
    /// This <see cref="Attribute"/> can be used on properties in a class inheriting from <see cref="ViewModel" />.
    /// </para>
    /// <para>
    /// It indicates, that the property depends on one or multiple properties.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertySourceAttribute
        : Attribute
    {
        /// <summary>
        /// Property names which should raise <see cref="INotifyPropertyChanged.PropertyChanged"/>.
        /// </summary>
        public IEnumerable<string> PropertySources { get; set; }

        /// <summary>
        /// Indicates if the attributes from the property of the base class should be considered.
        /// </summary>
        public bool InheritAttributes { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertySourceAttribute" /> class with one or multiple names of properties the property depends on.
        /// </summary>
        /// <param name="propertyNames">Names of source properties.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyNames"/> is <see langword="null"/>.</exception>
        public PropertySourceAttribute(params string[] propertyNames)
        {
            PropertySources = propertyNames ?? throw new ArgumentNullException(nameof(propertyNames));
        }
    }
}