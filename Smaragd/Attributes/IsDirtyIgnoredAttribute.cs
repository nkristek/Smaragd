using System;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Attributes
{
    /// <summary>
    /// This attribute can be used on properties in classes inheriting from <see cref="ViewModel"/>.
    /// <para />
    /// It indicates, that the property should not set <see cref="ViewModel.IsDirty"/> to <c>true</c> when it changes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IsDirtyIgnoredAttribute
        : Attribute
    {

    }
}
