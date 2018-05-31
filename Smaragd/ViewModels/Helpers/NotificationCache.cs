using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace NKristek.Smaragd.ViewModels.Helpers
{
    internal class NotificationCache
        : BindableBase, INotificationCache
    {
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

        /// <inheritdoc />
        public event ShouldNotifyCommandEventHandler ShouldNotifyCommand;

        /// <inheritdoc />
        public void AddPropertyNameToNotify(string propertyNameOfNotifyingProperty, string propertyNameToNotify)
        {
            if (!_propertiesNotifyingProperties.ContainsKey(propertyNameOfNotifyingProperty))
                _propertiesNotifyingProperties[propertyNameOfNotifyingProperty] = new List<string>();

            if (!_propertiesNotifyingProperties[propertyNameOfNotifyingProperty].Contains(propertyNameToNotify))
                _propertiesNotifyingProperties[propertyNameOfNotifyingProperty].Add(propertyNameToNotify);
        }

        /// <inheritdoc />
        public void AddCommandNameToNotify(string propertyNameOfNotifyingProperty, string commandNameToNotify)
        {
            if (!_propertiesNotifyingCommands.ContainsKey(propertyNameOfNotifyingProperty))
                _propertiesNotifyingCommands[propertyNameOfNotifyingProperty] = new List<string>();

            if (!_propertiesNotifyingCommands[propertyNameOfNotifyingProperty].Contains(commandNameToNotify))
                _propertiesNotifyingCommands[propertyNameOfNotifyingProperty].Add(commandNameToNotify);
        }

        /// <inheritdoc />
        public IEnumerable<string> GetPropertyNamesToNotify(string propertyName)
        {
            return _propertiesNotifyingProperties.ContainsKey(propertyName)
                ? _propertiesNotifyingProperties[propertyName]
                : Enumerable.Empty<string>();
        }

        /// <inheritdoc />
        public IEnumerable<string> GetCommandNamesToNotify(string propertyName)
        {
            return _propertiesNotifyingCommands.ContainsKey(propertyName)
                ? _propertiesNotifyingCommands[propertyName]
                : Enumerable.Empty<string>();
        }

        /// <inheritdoc />
        public void AddPropertyNameToNotify(string propertyNameOfNotifyingCollection, IEnumerable<NotifyCollectionChangedAction> collectionActions, string propertyNameToNotify)
        {
            if (!_collectionsNotifyingProperties.ContainsKey(propertyNameOfNotifyingCollection))
                _collectionsNotifyingProperties[propertyNameOfNotifyingCollection] = new List<Tuple<string, IList<NotifyCollectionChangedAction>>>();

            var propertiesToNotifyFromCollection = _collectionsNotifyingProperties[propertyNameOfNotifyingCollection];
            var propertyToNotifyWithActions = propertiesToNotifyFromCollection.SingleOrDefault(pa => pa.Item1 == propertyNameToNotify);
            if (propertyToNotifyWithActions == null)
            {
                propertyToNotifyWithActions = new Tuple<string, IList<NotifyCollectionChangedAction>>(propertyNameToNotify, new List<NotifyCollectionChangedAction>());
                propertiesToNotifyFromCollection.Add(propertyToNotifyWithActions);
            }

            foreach (var action in collectionActions.Where(a => !propertyToNotifyWithActions.Item2.Contains(a)))
                propertyToNotifyWithActions.Item2.Add(action);
        }

        /// <inheritdoc />
        public void AddCommandNameToNotify(string propertyNameOfNotifyingCollection, IEnumerable<NotifyCollectionChangedAction> collectionActions, string commandNameToNotify)
        {
            if (!_collectionsNotifyingCommands.ContainsKey(propertyNameOfNotifyingCollection))
                _collectionsNotifyingCommands[propertyNameOfNotifyingCollection] = new List<Tuple<string, IList<NotifyCollectionChangedAction>>>();

            var commandsToNotifyFromCollection = _collectionsNotifyingCommands[propertyNameOfNotifyingCollection];
            var commandToNotifyWithActions = commandsToNotifyFromCollection.SingleOrDefault(pa => pa.Item1 == commandNameToNotify);
            if (commandToNotifyWithActions == null)
            {
                commandToNotifyWithActions = new Tuple<string, IList<NotifyCollectionChangedAction>>(commandNameToNotify, new List<NotifyCollectionChangedAction>());
                commandsToNotifyFromCollection.Add(commandToNotifyWithActions);
            }

            foreach (var action in collectionActions.Where(a => !commandToNotifyWithActions.Item2.Contains(a)))
                commandToNotifyWithActions.Item2.Add(action);
        }

        /// <inheritdoc />
        public void RegisterCollection(INotifyCollectionChanged collection, string propertyName)
        {
            if (_registeredCollections.TryGetValue(propertyName, out var existingCollection) && existingCollection != collection)
                throw new Exception("There is already a collection registered for this name");

            if (!_registeredCollections.ContainsValue(collection))
                collection.CollectionChanged += OnCollectionChanged;

            _registeredCollections[propertyName] = collection;
        }

        /// <inheritdoc />
        public void UnregisterCollection(INotifyCollectionChanged collection, string propertyName)
        {
            if (!_registeredCollections.Remove(propertyName))
                throw new Exception("There is no collection registered for this name");

            if (!_registeredCollections.ContainsValue(collection))
                collection.CollectionChanged -= OnCollectionChanged;
        }

        private IEnumerable<string> GetPropertyNamesToNotify(INotifyCollectionChanged collection, NotifyCollectionChangedAction action)
        {
            var propertyNamesToNotify = new List<string>();
            foreach (var collectionName in _registeredCollections.Where(kvp => kvp.Value == collection).Select(kvp => kvp.Key))
            {
                if (!_collectionsNotifyingProperties.TryGetValue(collectionName, out var propertyNamesToNotifyFromCollection))
                    continue;

                foreach (var propertyNameToNotify in propertyNamesToNotifyFromCollection.Where(propertyActions => propertyActions.Item2.Contains(action)).Select(propertyActions => propertyActions.Item1))
                    propertyNamesToNotify.Add(propertyNameToNotify);
            }
            return propertyNamesToNotify.Distinct();
        }

        private IEnumerable<string> GetCommandNamesToNotify(INotifyCollectionChanged collection, NotifyCollectionChangedAction action)
        {
            var commandNamesToNotify = new List<string>();
            foreach (var collectionName in _registeredCollections.Where(kvp => kvp.Value == collection).Select(kvp => kvp.Key))
            {
                if (!_collectionsNotifyingCommands.TryGetValue(collectionName, out var commandNamesToNotifyFromCollection))
                    continue;

                foreach (var commandNameToNotify in commandNamesToNotifyFromCollection.Where(commandActions => commandActions.Item2.Contains(action)).Select(commandActions => commandActions.Item1))
                    commandNamesToNotify.Add(commandNameToNotify);
            }
            return commandNamesToNotify.Distinct();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var collection = sender as INotifyCollectionChanged;
            if (collection == null)
                return;

            foreach (var propertyNameToNotify in GetPropertyNamesToNotify(collection, e.Action))
                RaisePropertyChanged(propertyNameToNotify);

            foreach (var commandNameToNotify in GetCommandNamesToNotify(collection, e.Action))
                ShouldNotifyCommand?.Invoke(this, new ShouldNotifyCommandEventArgs(commandNameToNotify));
        }
    }
}
