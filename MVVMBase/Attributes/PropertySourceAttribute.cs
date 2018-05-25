using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using nkristek.MVVMBase.ViewModels;

namespace nkristek.MVVMBase.Attributes
{
    /// <summary>
    /// Use this on properties in classes that are subclasses of <see cref="ComputedBindableBase"/> to indicate, on which properties this property depends.
    /// It will then raise a PropertyChanged event for this property too.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertySourceAttribute
        : Attribute
    {
        public IEnumerable<string> PropertySources { get; }

        public string CollectionSource { get; }

        public IEnumerable<NotifyCollectionChangedAction> CollectionSourceActions { get; }

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
            CollectionSource = collectionName;
            CollectionSourceActions = actions;
        }
    }
}
