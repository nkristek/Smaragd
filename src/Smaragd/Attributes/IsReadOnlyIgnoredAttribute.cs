using System;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// This <see cref="Attribute"/> is used to indicate, that a property can still be set even when <see cref="ViewModel.IsReadOnly" /> is set to <see langword="true"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IsReadOnlyIgnoredAttribute
        : Attribute
    {
        /// <summary>
        /// Indicates if attributes of the property from the base class should be considered.
        /// </summary>
        public bool InheritAttributes { get; set; }
    }
}
