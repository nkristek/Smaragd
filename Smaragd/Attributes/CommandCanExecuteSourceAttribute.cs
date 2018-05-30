using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Attributes
{
    /// <summary>
    /// This attribute can be used on properties implementing <see cref="IRaiseCanExecuteChanged"/> in classes inheriting from <see cref="ComputedBindableBase"/>.
    /// <para />
    /// It indicates, that the <see cref="ICommand.CanExecute"/> method depends on a property or collection with this name.
    /// Given one or multiple property names, a <see cref="ICommand.CanExecuteChanged"/> event will be raised on this property, if a <see cref="INotifyPropertyChanged.PropertyChanged"/> event was raised for one of the specified property names.
    /// <para />
    /// To indicate that the source is a collection implementing <see cref="INotifyCollectionChanged"/> use <see cref="CommandCanExecuteSourceAttribute(string, NotifyCollectionChangedAction[])"/>.
    /// It will then raise a <see cref="ICommand.CanExecuteChanged"/> event on the property, once a <see cref="INotifyCollectionChanged.CollectionChanged"/> was raised on the source collection for one of the given <see cref="NotifyCollectionChangedAction"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CommandCanExecuteSourceAttribute
        : Attribute
    {
        public IEnumerable<string> PropertySources { get; }

        public Tuple<string, IList<NotifyCollectionChangedAction>> CollectionSource { get; }

        public CommandCanExecuteSourceAttribute(string propertyName)
        {
            // needed because of the CommandCanExecuteSourceAttribute(string collectionName, params NotifyCollectionChangedAction[] actions) overload
            PropertySources = Enumerable.Repeat(propertyName, 1);
        }

        public CommandCanExecuteSourceAttribute(params string[] propertyNames)
        {
            PropertySources = propertyNames;
        }

        public CommandCanExecuteSourceAttribute(string collectionName, params NotifyCollectionChangedAction[] actions)
        {
            CollectionSource = new Tuple<string, IList<NotifyCollectionChangedAction>>(collectionName, actions);
        }
    }
}
