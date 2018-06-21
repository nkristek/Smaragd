using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Attributes
{
    /// <summary>
    /// This attribute can be used on properties in classes inheriting from <see cref="ComputedBindableBase"/>.
    /// <para />
    /// It indicates, that the property depends a collection.
    /// <para />
    /// A <see cref="INotifyPropertyChanged.PropertyChanged"/> event will be raised for this property, once a <see cref="INotifyCollectionChanged.CollectionChanged"/> event was raised on the source collection with one of the given <see cref="NotifyCollectionChangedAction"/>.
    /// If no actions are provided, all <see cref="INotifyCollectionChanged.CollectionChanged"/> events will trigger a <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertySourceCollectionAttribute
        : Attribute
    {
        /// <summary>
        /// A tuple containing the source collection property name and a list of <see cref="NotifyCollectionChangedAction"/> which should trigger the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        public Tuple<string, IList<NotifyCollectionChangedAction>> CollectionSource { get; }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="collectionName">Name of the source collection property</param>
        /// <param name="actions">A list of <see cref="NotifyCollectionChangedAction"/> which should trigger the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.</param>
        public PropertySourceCollectionAttribute(string collectionName, params NotifyCollectionChangedAction[] actions)
        {
            CollectionSource = new Tuple<string, IList<NotifyCollectionChangedAction>>(collectionName, actions);
        }
    }
}
