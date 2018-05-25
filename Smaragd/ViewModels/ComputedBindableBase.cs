using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.Commands;

namespace NKristek.Smaragd.ViewModels
{
    /// <summary>
    /// Adds the functionality to use the <see cref="PropertySourceAttribute"/> above properties and <see cref="CommandCanExecuteSourceAttribute"/> above <see cref="IRaiseCanExecuteChanged"/> implementations
    /// </summary>
    public abstract class ComputedBindableBase
        : BindableBase
    {
        internal Dictionary<string, Tuple<PropertyInfo, IList<Attribute>>> CachedAttributes { get; } = new Dictionary<string, Tuple<PropertyInfo, IList<Attribute>>>();

        private readonly Dictionary<string, IList<string>> _propertiesNotifyingProperties = new Dictionary<string, IList<string>>();

        private readonly Dictionary<string, IList<string>> _propertiesNotifyingCommands = new Dictionary<string, IList<string>>();

        private readonly Dictionary<string, IList<Tuple<string, IList<NotifyCollectionChangedAction>>>> _collectionsNotifyingProperties = new Dictionary<string, IList<Tuple<string, IList<NotifyCollectionChangedAction>>>>();

        private readonly Dictionary<INotifyCollectionChanged, IList<Tuple<string, IList<NotifyCollectionChangedAction>>>> _cachedCollectionsNotifyingProperties = new Dictionary<INotifyCollectionChanged, IList<Tuple<string, IList<NotifyCollectionChangedAction>>>>();
        
        public ComputedBindableBase()
        {
            var properties = GetType().GetProperties().Where(p => p.GetMethod.IsPublic).ToList();
            foreach (var property in properties)
            {
                var customAttributes = property.GetCustomAttributes().ToList();
                if (customAttributes.Any())
                    CachedAttributes[property.Name] = new Tuple<PropertyInfo, IList<Attribute>>(property, customAttributes);
            }

            InitPropertyNamesToNotify(properties);
            PropertyChanged += NotifyPropertiesOnPropertyChanged;

            InitCommandNamesToNotify(properties);
            PropertyChanged += NotifyCommandsOnPropertyChanged;
        }
        
        private void InitPropertyNamesToNotify(IList<PropertyInfo> properties)
        {
            var propertyNames = properties.Select(p => p.Name).ToList();
            foreach (var propertyAttributes in CachedAttributes)
            {
                var propertyName = propertyAttributes.Key;
                var attributes = propertyAttributes.Value.Item2;
                foreach (var attribute in attributes.OfType<PropertySourceAttribute>())
                {
                    if (attribute.PropertySources != null)
                    {
                        // filter propertysources where the source is the name of the property itself or doesnt exist
                        AddPropertySources(propertyName, attribute.PropertySources.Where(ps => ps != propertyName && propertyNames.Contains(ps)));
                    }
                    else if (attribute.CollectionSource != null && attribute.CollectionSourceActions != null)
                    {
                        var collectionProperty = properties.FirstOrDefault(p => p.Name == attribute.CollectionSource);
                        if (collectionProperty == null)
                            throw new Exception($"The collection property called '{attribute.CollectionSource}' was not found.");
                        
                        if (!typeof(INotifyCollectionChanged).IsAssignableFrom(collectionProperty.PropertyType))
                            throw new Exception("Specifying a PropertySourceAttribute with a collection name and collection actions is not valid when the property does not implement INotifyCollectionChanged.");
                        
                        AddCollectionSource(propertyName, collectionProperty, attribute.CollectionSourceActions);
                    }
                }
            }

            // add collectionchanged event to initial collections
            foreach (var collectionNotifyingProperties in _collectionsNotifyingProperties)
            {
                var collectionProperty = properties.FirstOrDefault(p => p.Name == collectionNotifyingProperties.Key);
                if (collectionProperty == null)
                    continue;

                var collection = collectionProperty.GetValue(this, null) as INotifyCollectionChanged;
                if (collection == null)
                    continue;

                collection.CollectionChanged += OnCollectionChanged;
                _cachedCollectionsNotifyingProperties[collection] = collectionNotifyingProperties.Value;
            }
        }

        private void AddPropertySources(string propertyName, IEnumerable<string> propertySources)
        {
            foreach (var propertySource in propertySources)
            {
                if (!_propertiesNotifyingProperties.ContainsKey(propertySource))
                    _propertiesNotifyingProperties[propertySource] = new List<string>();

                if (!_propertiesNotifyingProperties[propertySource].Contains(propertyName))
                    _propertiesNotifyingProperties[propertySource].Add(propertyName);
            }
        }

        private void AddCollectionSource(string propertyName, PropertyInfo collectionProperty, IEnumerable<NotifyCollectionChangedAction> actions)
        {
            var collectionActions = actions.ToList();
            if (!collectionActions.Any())
                return;
            
            if (!_collectionsNotifyingProperties.TryGetValue(collectionProperty.Name, out var collectionNotifyingProperties))
            { 
                collectionNotifyingProperties = new List<Tuple<string, IList<NotifyCollectionChangedAction>>>();
                _collectionsNotifyingProperties[collectionProperty.Name] = collectionNotifyingProperties;
            }

            var propertyToNotify = collectionNotifyingProperties.FirstOrDefault(p => p.Item1 == propertyName);
            if (propertyToNotify == null)
            {
                collectionNotifyingProperties.Add(new Tuple<string, IList<NotifyCollectionChangedAction>>(propertyName, collectionActions));
            }
            else
            {
                var additionalActions = collectionActions.Where(a => !propertyToNotify.Item2.Contains(a));
                foreach (var additionalAction in additionalActions)
                    propertyToNotify.Item2.Add(additionalAction);
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var collection = sender as INotifyCollectionChanged;
            if (collection == null)
                return;

            if (!_cachedCollectionsNotifyingProperties.TryGetValue(collection, out var collectionPropertiesToNotify))
            {
                // this should not have happened, remove this method from the collectionchanged event to cleanup
                collection.CollectionChanged -= OnCollectionChanged;
                return;
            }
            
            foreach (var propertyToNotify in collectionPropertiesToNotify.Where(p => p.Item2.Contains(e.Action)).Select(p => p.Item1))
                RaisePropertyChanged(propertyToNotify);
        }
        
        private void InitCommandNamesToNotify(IList<PropertyInfo> properties)
        {
            var propertyNames = properties.Select(p => p.Name).ToList();
            foreach (var propertyAttributes in CachedAttributes.Where(a => typeof(IRaiseCanExecuteChanged).IsAssignableFrom(a.Value.Item1.PropertyType)))
            {
                var propertyName = propertyAttributes.Key;
                var property = propertyAttributes.Value.Item1;
                var attributes = propertyAttributes.Value.Item2;
                foreach (var attribute in attributes.OfType<CommandCanExecuteSourceAttribute>().Where(a => a.Sources != null))
                {
                    if (!typeof(IRaiseCanExecuteChanged).IsAssignableFrom(property.PropertyType))
                        throw new Exception("The property with the CommandCanExecuteSourceAttribute does not implement IRaiseCanExecuteChanged.");

                    // filter canexecutesources where the source is the name of the property itself or doesnt exist
                    AddCanExecuteSources(propertyName, attribute.Sources.Where(ps => ps != propertyName && propertyNames.Contains(ps)));
                }
            }
        }

        private void AddCanExecuteSources(string commandName, IEnumerable<string> canExecuteSources)
        {
            foreach (var sourceName in canExecuteSources)
            {
                if (!_propertiesNotifyingCommands.ContainsKey(sourceName))
                    _propertiesNotifyingCommands[sourceName] = new List<string>();

                if (!_propertiesNotifyingCommands[sourceName].Contains(commandName))
                    _propertiesNotifyingCommands[sourceName].Add(commandName);
            }
        }

        private void NotifyPropertiesOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_propertiesNotifyingProperties.TryGetValue(e.PropertyName, out var propertiesNotifyingProperties))
                return;

            foreach (var propertyNameToNotify in propertiesNotifyingProperties)
                RaisePropertyChanged(propertyNameToNotify);
        }

        private void NotifyCommandsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_propertiesNotifyingCommands.TryGetValue(e.PropertyName, out var propertiesNotifyingCommands))
                return;

            var type = GetType();
            foreach (var commandNameToNotify in propertiesNotifyingCommands)
            {
                try
                {
                    var value = type.GetProperty(commandNameToNotify)?.GetValue(this) as IRaiseCanExecuteChanged;
                    value?.RaiseCanExecuteChanged();
                }
                catch { }
            }
        }

        /// <summary>
        /// Sets a property value if the value is different and raises an event on the <see cref="PropertyChangedEventHandler"/>
        /// </summary>
        /// <typeparam name="T">Type of the property to set</typeparam>
        /// <param name="storage">Reference to the storage variable</param>
        /// <param name="value">New value to set</param>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="oldValue">The old value of the property</param>
        /// <returns>True if the value was different from the storage variable and the PropertyChanged event was raised</returns>
        protected override bool SetProperty<T>(ref T storage, T value, out T oldValue, [CallerMemberName] string propertyName = "")
        {
            var propertyWasChanged = base.SetProperty(ref storage, value, out oldValue, propertyName);
            if (propertyWasChanged)
            {
                if (oldValue is INotifyCollectionChanged oldCollection)
                {
                    oldCollection.CollectionChanged -= OnCollectionChanged;
                    _cachedCollectionsNotifyingProperties.Remove(oldCollection);
                }

                if (storage is INotifyCollectionChanged newCollection && _collectionsNotifyingProperties.TryGetValue(propertyName, out var propertyActions))
                {
                    newCollection.CollectionChanged += OnCollectionChanged;
                    _cachedCollectionsNotifyingProperties[newCollection] = propertyActions;
                }
            }
            return propertyWasChanged;
        }
    }
}
