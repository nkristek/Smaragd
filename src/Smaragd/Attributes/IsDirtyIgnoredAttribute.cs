using System;

namespace NKristek.Smaragd.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// This <see cref="Attribute"/> can be used on properties in classes inheriting from <see cref="T:NKristek.Smaragd.ViewModels.ViewModel" />.
    /// <para />
    /// It indicates, that a property should not set <see cref="P:NKristek.Smaragd.ViewModels.ViewModel.IsDirty" /> to <c>true</c> when the property changes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IsDirtyIgnoredAttribute
        : Attribute
    {
    }
}