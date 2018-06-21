using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.Commands;

namespace NKristek.Smaragd.ViewModels.Helpers
{
    internal sealed class NotificationCache
        : INotificationCache
    {
        private readonly Dictionary<string, IList<string>> _cachedPropertyNamesToNotify = new Dictionary<string, IList<string>>();

        private readonly Dictionary<string, IList<string>> _cachedCommandNamesToNotify = new Dictionary<string, IList<string>>();

        private readonly Dictionary<INotifyCollectionChanged, IList<string>> _cachedPropertyNamesToNotifyFromCollection = new Dictionary<INotifyCollectionChanged, IList<string>>();

        private readonly Dictionary<INotifyCollectionChanged, IList<string>> _cachedCommandNamesToNotifyFromCollection = new Dictionary<INotifyCollectionChanged, IList<string>>();

        /// <summary>
        /// Key is property name of notifying property, value is list of all properties to notify
        /// </summary>
        private readonly Dictionary<string, IList<string>> _propertiesNotifyingProperties = new Dictionary<string, IList<string>>();

        /// <summary>
        /// Key is property name of notifying property, value is list of all commands to notify
        /// </summary>
        private readonly Dictionary<string, IList<string>> _propertiesNotifyingCommands = new Dictionary<string, IList<string>>();

        /// <summary>
        /// Key is property name of notifying collection, value is list of all properties to notify with collection actions
        /// </summary>
        private readonly Dictionary<string, IList<Tuple<string, IList<NotifyCollectionChangedAction>>>> _collectionsNotifyingProperties = new Dictionary<string, IList<Tuple<string, IList<NotifyCollectionChangedAction>>>>();

        /// <summary>
        /// Key is property name of notifying collection, value is list of all commands to notify with collection actions
        /// </summary>
        private readonly Dictionary<string, IList<Tuple<string, IList<NotifyCollectionChangedAction>>>> _collectionsNotifyingCommands = new Dictionary<string, IList<Tuple<string, IList<NotifyCollectionChangedAction>>>>();

        /// <summary>
        /// Key is property name of collection, value is the collection itself
        /// </summary>
        private readonly Dictionary<string, INotifyCollectionChanged> _registeredCollections = new Dictionary<string, INotifyCollectionChanged>();

        private WeakReference<ComputedBindableBase> _parent;
        
        private ComputedBindableBase Parent
        {
            get
            {
                if (_parent != null && _parent.TryGetTarget(out var parent))
                    return parent;
                return null;
            }

            set
            {
                if (Parent == value) return;
                _parent = value != null ? new WeakReference<ComputedBindableBase>(value) : null;
            }
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler ShouldNotifyProperty;

        /// <inheritdoc />
        public event ShouldNotifyCommandEventHandler ShouldNotifyCommand;

        internal NotificationCache(ComputedBindableBase computedBindableBase)
        {
            Parent = computedBindableBase ?? throw new ArgumentNullException(nameof(computedBindableBase));

            var properties = computedBindableBase.GetType().GetProperties().Where(p => p.GetMethod.IsPublic).ToList();
            InitPropertiesToNotify(properties);
            InitCommandNamesToNotify(properties);
        }

        private void InitPropertiesToNotify(IList<PropertyInfo> properties)
        {
            var parent = Parent;
            if (parent == null)
                return;

            var propertyNames = properties.Select(p => p.Name).ToList();
            foreach (var propertyAttributes in parent.CachedAttributes)
            {
                var propertyName = propertyAttributes.Key;
                var attributes = propertyAttributes.Value.Item2;

                foreach (var attribute in attributes.OfType<PropertySourceAttribute>().Where(a => a.PropertySources != null))
                {
                    // filter propertysources where the source is the name of the property itself or doesnt exist
                    foreach (var propertySource in attribute.PropertySources.Where(ps => ps != propertyName && propertyNames.Contains(ps)))
                        AddPropertyNameToNotify(propertySource, propertyName);
                }

                foreach (var attribute in attributes.OfType<PropertySourceCollectionAttribute>().Where(a => a.CollectionSource != null))
                {
                    var collectionName = attribute.CollectionSource.Item1;
                    var collectionActions = attribute.CollectionSource.Item2;
                    
                    if (String.IsNullOrEmpty(collectionName))
                        throw new Exception("The collection name is null or empty.");
                    
                    var collectionProperty = properties.FirstOrDefault(p => p.Name == collectionName);
                    if (collectionProperty == null)
                        throw new Exception($"The collection property called '{attribute.CollectionSource}' was not found.");
                    
                    if (!typeof(INotifyCollectionChanged).IsAssignableFrom(collectionProperty.PropertyType))
                        throw new Exception("Specifying a PropertySourceCollectionAttribute with a collection name and collection actions is not valid when the collection property does not implement INotifyCollectionChanged.");

                    AddPropertyNameToNotify(collectionName, collectionActions ?? new List<NotifyCollectionChangedAction>(), propertyName);

                    var collection = collectionProperty.GetValue(parent, null) as INotifyCollectionChanged;
                    if (collection == null)
                        continue;

                    RegisterCollection(collection, collectionName);
                }
            }
        }

        private void InitCommandNamesToNotify(IList<PropertyInfo> properties)
        {
            var parent = Parent;
            if (parent == null)
                return;

            var propertyNames = properties.Select(p => p.Name).ToList();
            foreach (var propertyAttributes in parent.CachedAttributes.Where(ca => ca.Value.Item2.Any(a => a is CommandCanExecuteSourceAttribute)))
            {
                var propertyName = propertyAttributes.Key;
                var property = propertyAttributes.Value.Item1;
                var attributes = propertyAttributes.Value.Item2;

                if (!typeof(IRaiseCanExecuteChanged).IsAssignableFrom(property.PropertyType))
                    throw new Exception($"The property {propertyName} does not implement IRaiseCanExecuteChanged and thus can not use this attribute.");

                foreach (var attribute in attributes.OfType<CommandCanExecuteSourceAttribute>().Where(a => a.PropertySources != null))
                {
                    // filter propertysources where the source is the name of the property itself or doesnt exist
                    foreach (var propertySource in attribute.PropertySources.Where(ps => ps != propertyName && propertyNames.Contains(ps)))
                        AddCommandNameToNotify(propertySource, propertyName);
                }

                foreach (var attribute in attributes.OfType<CommandCanExecuteSourceCollectionAttribute>().Where(a => a.CollectionSource != null))
                {
                    var collectionName = attribute.CollectionSource.Item1;
                    var collectionActions = attribute.CollectionSource.Item2;

                    if (String.IsNullOrEmpty(collectionName))
                        throw new Exception("The collection name is null or empty.");

                    var collectionProperty = properties.FirstOrDefault(p => p.Name == collectionName);
                    if (collectionProperty == null)
                        throw new Exception($"The collection property called '{attribute.CollectionSource}' was not found.");

                    if (!typeof(INotifyCollectionChanged).IsAssignableFrom(collectionProperty.PropertyType))
                        throw new Exception("Specifying a CommandCanExecuteSourceCollectionAttribute with a collection name and collection actions is not valid when the collection property does not implement INotifyCollectionChanged.");

                    AddCommandNameToNotify(collectionName, collectionActions ?? new List<NotifyCollectionChangedAction>(), propertyName);

                    var collection = collectionProperty.GetValue(parent, null) as INotifyCollectionChanged;
                    if (collection == null)
                        continue;

                    RegisterCollection(collection, collectionName);
                }
            }
        }
        
        /// <inheritdoc />
        public void AddPropertyNameToNotify(string propertyNameOfNotifyingProperty, string propertyNameToNotify)
        {
            InvalidateCache();

            if (!_propertiesNotifyingProperties.ContainsKey(propertyNameOfNotifyingProperty))
                _propertiesNotifyingProperties[propertyNameOfNotifyingProperty] = new List<string>();

            if (!_propertiesNotifyingProperties[propertyNameOfNotifyingProperty].Contains(propertyNameToNotify))
                _propertiesNotifyingProperties[propertyNameOfNotifyingProperty].Add(propertyNameToNotify);
        }

        /// <inheritdoc />
        public void AddCommandNameToNotify(string propertyNameOfNotifyingProperty, string commandNameToNotify)
        {
            InvalidateCache();

            if (!_propertiesNotifyingCommands.ContainsKey(propertyNameOfNotifyingProperty))
                _propertiesNotifyingCommands[propertyNameOfNotifyingProperty] = new List<string>();

            if (!_propertiesNotifyingCommands[propertyNameOfNotifyingProperty].Contains(commandNameToNotify))
                _propertiesNotifyingCommands[propertyNameOfNotifyingProperty].Add(commandNameToNotify);
        }

        private void InvalidateCache()
        {
            _cachedPropertyNamesToNotify.Clear();
            _cachedCommandNamesToNotify.Clear();
            _cachedPropertyNamesToNotifyFromCollection.Clear();
            _cachedCommandNamesToNotifyFromCollection.Clear();
        }

        /// <inheritdoc />
        public IEnumerable<string> GetPropertyNamesToNotify(string propertyName)
        {
            if (_cachedPropertyNamesToNotify.TryGetValue(propertyName, out var cachedPropertyNamesToNotify))
                return cachedPropertyNamesToNotify.ToList();
            _cachedPropertyNamesToNotify[propertyName] = GetRecursivePropertyNamesToNotify(propertyName, new List<string>()).ToList();
            _cachedPropertyNamesToNotify[propertyName].Remove(propertyName);
            return _cachedPropertyNamesToNotify[propertyName].ToList();
        }

        private IEnumerable<string> GetRecursivePropertyNamesToNotify(string propertyName, IList<string> propertyNamesToNotify)
        {
            if (String.IsNullOrEmpty(propertyName) || !_propertiesNotifyingProperties.TryGetValue(propertyName, out var currentPropertyNamesToNotify))
                return propertyNamesToNotify;
            
            foreach (var propertyNameToNotify in currentPropertyNamesToNotify)
            {
                if (propertyNamesToNotify.Contains(propertyNameToNotify))
                    continue;

                propertyNamesToNotify.Add(propertyNameToNotify);
                GetRecursivePropertyNamesToNotify(propertyNameToNotify, propertyNamesToNotify);
            }

            return propertyNamesToNotify;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetCommandNamesToNotify(string propertyName)
        {
            if (_cachedCommandNamesToNotify.TryGetValue(propertyName, out var cachedCommandNamesToNotify))
                return cachedCommandNamesToNotify.ToList();

            var commandNamesToNotify = new List<string>();

            if (_propertiesNotifyingCommands.TryGetValue(propertyName, out var currentCommandNamesToNotify))
            {
                foreach (var commandNameToNotify in currentCommandNamesToNotify)
                {
                    if (!commandNamesToNotify.Contains(commandNameToNotify))
                        commandNamesToNotify.Add(commandNameToNotify);
                }
            }

            var propertyNamesToNotify = GetPropertyNamesToNotify(propertyName);
            foreach (var recursiveCommandNamesToNotify in propertyNamesToNotify
                .Where(name => _propertiesNotifyingCommands.ContainsKey(name))
                .Select(name => _propertiesNotifyingCommands[name]))
            {
                foreach (var commandNameToNotify in recursiveCommandNamesToNotify)
                {
                    if (!commandNamesToNotify.Contains(commandNameToNotify))
                        commandNamesToNotify.Add(commandNameToNotify);
                }
            }

            _cachedCommandNamesToNotify[propertyName] = commandNamesToNotify;
            return _cachedCommandNamesToNotify[propertyName].ToList();
        }

        /// <inheritdoc />
        public void AddPropertyNameToNotify(string propertyNameOfNotifyingCollection, IEnumerable<NotifyCollectionChangedAction> collectionActions, string propertyNameToNotify)
        {
            InvalidateCache();
            AddPropertyNameToNotify(propertyNameOfNotifyingCollection, propertyNameToNotify);

            if (!_collectionsNotifyingProperties.ContainsKey(propertyNameOfNotifyingCollection))
                _collectionsNotifyingProperties[propertyNameOfNotifyingCollection] = new List<Tuple<string, IList<NotifyCollectionChangedAction>>>();

            var propertiesToNotifyFromCollection = _collectionsNotifyingProperties[propertyNameOfNotifyingCollection];
            var propertyToNotifyWithActions = propertiesToNotifyFromCollection.SingleOrDefault(pa => pa.Item1 == propertyNameToNotify);
            if (propertyToNotifyWithActions == null)
            {
                propertyToNotifyWithActions = new Tuple<string, IList<NotifyCollectionChangedAction>>(propertyNameToNotify, new List<NotifyCollectionChangedAction>(collectionActions));
                propertiesToNotifyFromCollection.Add(propertyToNotifyWithActions);
            }
            else
            {
                foreach (var action in collectionActions.Where(a => !propertyToNotifyWithActions.Item2.Contains(a)))
                    propertyToNotifyWithActions.Item2.Add(action);
            }
        }

        /// <inheritdoc />
        public void AddCommandNameToNotify(string propertyNameOfNotifyingCollection, IEnumerable<NotifyCollectionChangedAction> collectionActions, string commandNameToNotify)
        {
            InvalidateCache();
            AddCommandNameToNotify(propertyNameOfNotifyingCollection, commandNameToNotify);

            if (!_collectionsNotifyingCommands.ContainsKey(propertyNameOfNotifyingCollection))
                _collectionsNotifyingCommands[propertyNameOfNotifyingCollection] = new List<Tuple<string, IList<NotifyCollectionChangedAction>>>();

            var commandsToNotifyFromCollection = _collectionsNotifyingCommands[propertyNameOfNotifyingCollection];
            var commandToNotifyWithActions = commandsToNotifyFromCollection.SingleOrDefault(pa => pa.Item1 == commandNameToNotify);
            if (commandToNotifyWithActions == null)
            {
                commandToNotifyWithActions = new Tuple<string, IList<NotifyCollectionChangedAction>>(commandNameToNotify, new List<NotifyCollectionChangedAction>(collectionActions));
                commandsToNotifyFromCollection.Add(commandToNotifyWithActions);
            }
            else
            {
                foreach (var action in collectionActions.Where(a => !commandToNotifyWithActions.Item2.Contains(a)))
                    commandToNotifyWithActions.Item2.Add(action);
            }
        }

        /// <inheritdoc />
        public void RegisterCollection(INotifyCollectionChanged collection, string propertyName)
        {
            if (_registeredCollections.TryGetValue(propertyName, out var existingCollection))
            {
                if (existingCollection != collection)
                    throw new Exception("There is already a collection registered for this name");
                return;
            }

            collection.CollectionChanged += OnCollectionChanged;
            _registeredCollections[propertyName] = collection;
        }

        /// <inheritdoc />
        public void UnregisterCollection(INotifyCollectionChanged collection, string propertyName)
        {
            if (!_registeredCollections.Remove(propertyName))
                throw new Exception("There is no collection registered for this name");

            collection.CollectionChanged -= OnCollectionChanged;
        }

        private IEnumerable<string> GetPropertyNamesToNotify(INotifyCollectionChanged collection, NotifyCollectionChangedAction action)
        {
            if (_cachedPropertyNamesToNotifyFromCollection.TryGetValue(collection, out var cachedPropertyNamesToNotify))
                return cachedPropertyNamesToNotify.ToList();

            var propertyNamesToNotify = new List<string>();
            foreach (var collectionName in _registeredCollections.Where(kvp => kvp.Value == collection).Select(kvp => kvp.Key))
            {
                if (!_collectionsNotifyingProperties.TryGetValue(collectionName, out var propertyNamesToNotifyFromCollection))
                    continue;

                foreach (var propertyNameToNotify in propertyNamesToNotifyFromCollection.Where(propertyActions => !propertyActions.Item2.Any() || propertyActions.Item2.Contains(action)).Select(propertyActions => propertyActions.Item1))
                    if (!propertyNamesToNotify.Contains(propertyNameToNotify))
                        propertyNamesToNotify.Add(propertyNameToNotify);
            }

            _cachedPropertyNamesToNotifyFromCollection[collection] = propertyNamesToNotify;
            return _cachedPropertyNamesToNotifyFromCollection[collection].ToList();
        }

        private IEnumerable<string> GetCommandNamesToNotify(INotifyCollectionChanged collection, NotifyCollectionChangedAction action)
        {
            if (_cachedCommandNamesToNotifyFromCollection.TryGetValue(collection, out var currentCommandNamesToNotify))
                return currentCommandNamesToNotify.ToList();

            var commandNamesToNotify = new List<string>();
            foreach (var collectionName in _registeredCollections.Where(kvp => kvp.Value == collection).Select(kvp => kvp.Key))
            {
                if (!_collectionsNotifyingCommands.TryGetValue(collectionName, out var commandNamesToNotifyFromCollection))
                    continue;

                foreach (var commandNameToNotify in commandNamesToNotifyFromCollection.Where(commandActions => !commandActions.Item2.Any() || commandActions.Item2.Contains(action)).Select(commandActions => commandActions.Item1))
                    if (!commandNamesToNotify.Contains(commandNameToNotify))
                        commandNamesToNotify.Add(commandNameToNotify);
            }

            _cachedCommandNamesToNotifyFromCollection[collection] = commandNamesToNotify;
            return _cachedCommandNamesToNotifyFromCollection[collection].ToList();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var collection = sender as INotifyCollectionChanged;
            if (collection == null)
                return;

            foreach (var propertyNameToNotify in GetPropertyNamesToNotify(collection, e.Action))
                ShouldNotifyProperty?.Invoke(this, new PropertyChangedEventArgs(propertyNameToNotify));

            foreach (var commandNameToNotify in GetCommandNamesToNotify(collection, e.Action))
                ShouldNotifyCommand?.Invoke(this, new ShouldNotifyCommandEventArgs(commandNameToNotify));
        }
    }
}
