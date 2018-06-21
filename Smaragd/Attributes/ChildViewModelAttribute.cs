using System;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Attributes
{
    /// <summary>
    /// This attribute indicates that the property is a <see cref="ViewModel"/> and should be added to <see cref="ViewModel.Children"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ChildViewModelAttribute
        : Attribute
    {
    }
}
