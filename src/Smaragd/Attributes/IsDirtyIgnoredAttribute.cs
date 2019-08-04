using System;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// This <see cref="Attribute"/> is used to indicate, that the <see cref="ViewModel.IsDirty" /> property should not be set to <see langword="true"/> when the property value changes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IsDirtyIgnoredAttribute
        : Attribute
    {
        /// <summary>
        /// Indicates if attributes of the property from the base class should be considered.
        /// </summary>
        public bool InheritAttributes { get; set; }
    }
}