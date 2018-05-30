using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.ViewModels.Helpers;

namespace NKristek.Smaragd.ViewModels
{
    /// <summary>
    /// This class adds support to use the <see cref="PropertySourceAttribute"/> above properties and <see cref="CommandCanExecuteSourceAttribute"/> above <see cref="IRaiseCanExecuteChanged"/> implementations.
    /// </summary>
    public abstract class ComputedBindableBase
        : BindableBase
    {
        internal Dictionary<string, Tuple<PropertyInfo, IList<Attribute>>> CachedAttributes { get; } = new Dictionary<string, Tuple<PropertyInfo, IList<Attribute>>>();

        private readonly INotificationCache _notificationCache;

        protected ComputedBindableBase()
        {
            var notificationCache = new NotificationCache();
            notificationCache.PropertyChanged += (sender, args) => { RaisePropertyChanged(args.PropertyName); };
            notificationCache.ShouldNotifyCommand += (sender, args) => { RaiseCommandCanExecuteChanged(args.CommandNameToNotify); };
            _notificationCache = notificationCache;

            var properties = GetType().GetProperties().Where(p => p.GetMethod.IsPublic).ToList();
            foreach (var property in properties)
            {
                var customAttributes = property.GetCustomAttributes().ToList();
                if (customAttributes.Any())
                    CachedAttributes[property.Name] = new Tuple<PropertyInfo, IList<Attribute>>(property, customAttributes);
            }

            InitPropertiesToNotify(properties);
            InitCommandNamesToNotify(properties);
        }
        
        private void InitPropertiesToNotify(IList<PropertyInfo> properties)
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
                        foreach (var propertySource in attribute.PropertySources.Where(ps => ps != propertyName && propertyNames.Contains(ps)))
                            _notificationCache.AddPropertyNameToNotify(propertySource, propertyName);
                    }
                    else if (attribute.CollectionSource != null)
                    {
                        var collectionName = attribute.CollectionSource.Item1;
                        var collectionActions = attribute.CollectionSource.Item2;
                        if (!collectionActions.Any())
                            continue;

                        if (String.IsNullOrEmpty(collectionName))
                            throw new Exception("The collection name is null or empty.");

                        var collectionProperty = properties.FirstOrDefault(p => p.Name == collectionName);
                        if (collectionProperty == null)
                            throw new Exception($"The collection property called '{attribute.CollectionSource}' was not found.");
                        
                        if (!typeof(INotifyCollectionChanged).IsAssignableFrom(collectionProperty.PropertyType))
                            throw new Exception("Specifying a PropertySourceAttribute with a collection name and collection actions is not valid when the property does not implement INotifyCollectionChanged.");
                        
                        _notificationCache.AddPropertyNameToNotify(collectionName, collectionActions, propertyName);

                        var collection = collectionProperty.GetValue(this, null) as INotifyCollectionChanged;
                        if (collection == null)
                            continue;

                        _notificationCache.RegisterCollection(collection, collectionName);
                    }
                }
            }
        }
        
        private void InitCommandNamesToNotify(IList<PropertyInfo> properties)
        {
            var propertyNames = properties.Select(p => p.Name).ToList();
            foreach (var propertyAttributes in CachedAttributes.Where(ca => ca.Value.Item2.Any(a => a is CommandCanExecuteSourceAttribute)))
            {
                var propertyName = propertyAttributes.Key;
                var property = propertyAttributes.Value.Item1;
                var attributes = propertyAttributes.Value.Item2;

                if (!typeof(IRaiseCanExecuteChanged).IsAssignableFrom(property.PropertyType))
                    throw new Exception("The property with the CommandCanExecuteSourceAttribute does not implement IRaiseCanExecuteChanged.");

                foreach (var attribute in attributes.OfType<CommandCanExecuteSourceAttribute>())
                {
                    if (attribute.PropertySources != null)
                    {
                        // filter propertysources where the source is the name of the property itself or doesnt exist
                        foreach (var propertySource in attribute.PropertySources.Where(ps => ps != propertyName && propertyNames.Contains(ps)))
                            _notificationCache.AddCommandNameToNotify(propertySource, propertyName);
                    }
                    else if (attribute.CollectionSource != null)
                    {
                        var collectionName = attribute.CollectionSource.Item1;
                        var collectionActions = attribute.CollectionSource.Item2;
                        if (!collectionActions.Any())
                            continue;

                        if (String.IsNullOrEmpty(collectionName))
                            throw new Exception("The collection name is null or empty.");

                        var collectionProperty = properties.FirstOrDefault(p => p.Name == collectionName);
                        if (collectionProperty == null)
                            throw new Exception($"The collection property called '{attribute.CollectionSource}' was not found.");

                        if (!typeof(INotifyCollectionChanged).IsAssignableFrom(collectionProperty.PropertyType))
                            throw new Exception("Specifying a PropertySourceAttribute with a collection name and collection actions is not valid when the property does not implement INotifyCollectionChanged.");

                        _notificationCache.AddCommandNameToNotify(collectionName, collectionActions, propertyName);

                        var collection = collectionProperty.GetValue(this, null) as INotifyCollectionChanged;
                        if (collection == null)
                            continue;

                        _notificationCache.RegisterCollection(collection, collectionName);
                    }
                }
            }
        }
        
        /// <inheritdoc />
        protected override bool SetProperty<T>(ref T storage, T value, out T oldValue, [CallerMemberName] string propertyName = "")
        {
            var propertyWasChanged = base.SetProperty(ref storage, value, out oldValue, propertyName);
            if (propertyWasChanged)
            {
                if (oldValue is INotifyCollectionChanged oldCollection)
                    _notificationCache.UnregisterCollection(oldCollection, propertyName);

                if (storage is INotifyCollectionChanged newCollection)
                    _notificationCache.RegisterCollection(newCollection, propertyName);
            }
            return propertyWasChanged;
        }

        internal override bool InternalRaisePropertyChanged(string propertyName)
        {
            var propertyChangedWasRaised = base.InternalRaisePropertyChanged(propertyName);
            if (propertyChangedWasRaised)
            {
                NotifyPropertiesOnPropertyChanged(propertyName);
                NotifyCommandsOnPropertyChanged(propertyName);
            }
            return propertyChangedWasRaised;
        }

        private void NotifyPropertiesOnPropertyChanged(string propertyName)
        {
            foreach (var propertyNameToNotify in _notificationCache.GetPropertyNamesToNotify(propertyName))
                RaisePropertyChanged(propertyNameToNotify);
        }

        private void NotifyCommandsOnPropertyChanged(string propertyName)
        {
            foreach (var commandNameToNotify in _notificationCache.GetCommandNamesToNotify(propertyName))
                RaiseCommandCanExecuteChanged(commandNameToNotify);
        }

        private void RaiseCommandCanExecuteChanged(string commandName)
        {
            try
            {
                var value = GetType().GetProperty(commandName)?.GetValue(this, null) as IRaiseCanExecuteChanged;
                value?.RaiseCanExecuteChanged();
            }
            catch { }
        }
    }
}
