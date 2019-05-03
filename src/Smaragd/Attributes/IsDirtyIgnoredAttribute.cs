using System;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// <para>
    /// This <see cref="Attribute"/> can be used on properties in classes inheriting from <see cref="ViewModel" />.
    /// </para>
    /// <para>
    /// It indicates, that a property should not set <see cref="ViewModel.IsDirty" /> to <see langword="true"/> when the property changes.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IsDirtyIgnoredAttribute
        : Attribute
    {
        /// <summary>
        /// Indicates if the attributes from the base class should be considered.
        /// </summary>
        public bool InheritAttributes { get; set; }
    }
}