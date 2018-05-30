using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Attributes
{
    /// <summary>
    /// This attribute can be used on properties in classes inheriting from <see cref="ComputedBindableBase"/>.
    /// <para />
    /// It indicates, that the property depends on other properties or a collection.
    /// Given one or multiple property names, an additional <see cref="INotifyPropertyChanged.PropertyChanged"/> event will be raised for this property, if one was raised for one of the specified property names.
    /// <para />
    /// To indicate that the source is a collection implementing <see cref="INotifyCollectionChanged"/> use <see cref="PropertySourceAttribute(string, NotifyCollectionChangedAction[])"/> constructor.
    /// It will then raise a <see cref="INotifyPropertyChanged.PropertyChanged"/> event for this property, once a <see cref="INotifyCollectionChanged.CollectionChanged"/> event was raised on the source collection with one of the given <see cref="NotifyCollectionChangedAction"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertySourceAttribute
        : Attribute
    {
        public IEnumerable<string> PropertySources { get; }
        
        public Tuple<string, IList<NotifyCollectionChangedAction>> CollectionSource { get; }

        public PropertySourceAttribute(string propertyName)
        {
            // needed because of the PropertySourceAttribute(string collectionName, params NotifyCollectionChangedAction[] actions) overload
            PropertySources = Enumerable.Repeat(propertyName, 1);
        }

        public PropertySourceAttribute(params string[] propertyNames)
        {
            PropertySources = propertyNames;
        }
        
        public PropertySourceAttribute(string collectionName, params NotifyCollectionChangedAction[] actions)
        {
            CollectionSource = new Tuple<string, IList<NotifyCollectionChangedAction>>(collectionName, actions);
        }
    }
}
