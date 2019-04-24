using System;

namespace NKristek.Smaragd.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// This <see cref="Attribute"/> can be used on properties in classes inheriting from <see cref="T:NKristek.Smaragd.ViewModels.ViewModel" />.
    /// <para />
    /// It indicates, that a property can still be set even when <see cref="P:NKristek.Smaragd.ViewModels.ViewModel.IsReadOnly" /> is set to <c>true</c>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IsReadOnlyIgnoredAttribute
        : Attribute
    {
        /// <summary>
        /// Indicates if the attributes from the base class should be considered.
        /// </summary>
        public bool InheritAttributes { get; set; }
    }
}
