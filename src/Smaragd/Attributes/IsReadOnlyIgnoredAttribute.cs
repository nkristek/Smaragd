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
    /// It indicates, that a property can still be set even when <see cref="ViewModel.IsReadOnly" /> is set to <see langword="true"/>.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IsReadOnlyIgnoredAttribute
        : Attribute
    {
        /// <summary>
        /// Indicates if the attributes from the property of the base class should be considered.
        /// </summary>
        public bool InheritAttributes { get; set; }
    }
}
