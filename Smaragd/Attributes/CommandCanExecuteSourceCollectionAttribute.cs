using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Input;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Attributes
{
    /// <summary>
    /// This attribute can be used on properties implementing <see cref="IRaiseCanExecuteChanged"/> in classes inheriting from <see cref="ComputedBindableBase"/>.
    /// <para />
    /// It indicates, that the <see cref="ICommand.CanExecute"/> method depends on a collection.
    /// <para />
    /// A <see cref="ICommand.CanExecuteChanged"/> event will be raised on the command, once a <see cref="INotifyCollectionChanged.CollectionChanged"/> was raised on the source collection for one of the given <see cref="NotifyCollectionChangedAction"/>.
    /// If no actions are provided, all <see cref="INotifyCollectionChanged.CollectionChanged"/> events will trigger a <see cref="ICommand.CanExecuteChanged"/> event.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CommandCanExecuteSourceCollectionAttribute
        : Attribute
    {
        /// <summary>
        /// A tuple containing the source collection property name and a list of <see cref="NotifyCollectionChangedAction"/> which should trigger the <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        public Tuple<string, IList<NotifyCollectionChangedAction>> CollectionSource { get; }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="collectionName">Name of the source collection property</param>
        /// <param name="actions">A list of <see cref="NotifyCollectionChangedAction"/> which should trigger the <see cref="ICommand.CanExecuteChanged"/> event.</param>
        public CommandCanExecuteSourceCollectionAttribute(string collectionName, params NotifyCollectionChangedAction[] actions)
        {
            CollectionSource = new Tuple<string, IList<NotifyCollectionChangedAction>>(collectionName, actions);
        }
    }
}
