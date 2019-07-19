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
    /// It indicates, that <see cref="ViewModel.IsDirty" /> should not be set to <see langword="true"/> when the property value changes.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IsDirtyIgnoredAttribute
        : Attribute
    {
        /// <summary>
        /// Indicates if the attributes from the property of the base class should be considered.
        /// </summary>
        public bool InheritAttributes { get; set; }
    }
}