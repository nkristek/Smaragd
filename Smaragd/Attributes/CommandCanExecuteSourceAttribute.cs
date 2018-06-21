using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Attributes
{
    /// <summary>
    /// This attribute can be used on properties implementing <see cref="IRaiseCanExecuteChanged"/> in classes inheriting from <see cref="ComputedBindableBase"/>.
    /// <para />
    /// It indicates, that the <see cref="ICommand.CanExecute"/> method depends on one or multiple properties.
    /// Given one or multiple property names, a <see cref="ICommand.CanExecuteChanged"/> event will be raised on this property, if a <see cref="INotifyPropertyChanged.PropertyChanged"/> event was raised for one of the specified property names.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CommandCanExecuteSourceAttribute
        : Attribute
    {
        public IEnumerable<string> PropertySources { get; }
        
        public CommandCanExecuteSourceAttribute(params string[] propertyNames)
        {
            PropertySources = propertyNames;
        }
    }
}
