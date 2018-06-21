using System;
using System.Collections.Specialized;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Attributes
{
    /// <summary>
    /// This attribute indicates that the property is a <see cref="ViewModel"/> collection implementing <see cref="INotifyCollectionChanged"/> and items of this collection should be added to <see cref="ViewModel.Children"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ChildViewModelCollectionAttribute
        : Attribute
    {
    }
}
