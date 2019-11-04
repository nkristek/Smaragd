using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NKristek.Smaragd.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// This <see cref="Attribute"/> is used to indicate, that the property depends on one or multiple properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertySourceAttribute
        : Attribute
    {
        /// <summary>
        /// Property names which should raise <see cref="INotifyPropertyChanging.PropertyChanging"/> and <see cref="INotifyPropertyChanged.PropertyChanged"/>.
        /// </summary>
        public IEnumerable<string> PropertySources { get; set; }

        /// <summary>
        /// Indicates if attributes of the property from the base class should be considered.
        /// </summary>
        public bool InheritAttributes { get; set; }

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