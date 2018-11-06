using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Attributes
{
    /// <summary>
    /// This attribute can be used on <see cref="ICommand.CanExecute"/> in a class implementing <see cref="IRaiseCanExecuteChanged"/>.
    /// <para />
    /// It indicates, that <see cref="ICommand.CanExecute"/> depends on one or multiple properties in the parent <see cref="ViewModel"/>.
    /// <para />
    /// Given one or multiple properties with the specified names raise an event on <see cref="INotifyPropertyChanged.PropertyChanged"/> of the parent <see cref="ViewModel"/>, a <see cref="ICommand.CanExecuteChanged"/> event will be raised as well.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CanExecuteSourceAttribute
        : Attribute
    {
        /// <summary>
        /// Property names of source properties
        /// </summary>
        public IEnumerable<string> PropertySources { get; }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="propertyNames">Property names of source properties</param>
        public CanExecuteSourceAttribute(params string[] propertyNames)
        {
            PropertySources = propertyNames;
        }
    }
}
