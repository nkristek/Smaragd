using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public Tuple<string, IList<NotifyCollectionChangedAction>> CollectionSource { get; }
        
        public CommandCanExecuteSourceCollectionAttribute(string collectionName, params NotifyCollectionChangedAction[] actions)
        {
            CollectionSource = new Tuple<string, IList<NotifyCollectionChangedAction>>(collectionName, actions);
        }
    }
}
