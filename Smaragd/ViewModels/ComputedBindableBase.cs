using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.ViewModels.Helpers;

namespace NKristek.Smaragd.ViewModels
{
    /// <summary>
    /// This class adds support to use the <see cref="PropertySourceAttribute"/> and <see cref="PropertySourceCollectionAttribute"/> above properties and <see cref="CommandCanExecuteSourceAttribute"/> and <see cref="CommandCanExecuteSourceCollectionAttribute"/> above <see cref="IRaiseCanExecuteChanged"/> implementations.
    /// </summary>
    public abstract class ComputedBindableBase
        : BindableBase
    {
        private INotificationCache _notificationCache;

        private INotificationCache NotificationCache
        {
            get => _notificationCache;
            set
            {
                if (_notificationCache != null)
                {
                    _notificationCache.ShouldNotifyProperty -= NotificationCacheOnShouldNotifyProperty;
                    _notificationCache.ShouldNotifyCommand -= NotificationCacheOnShouldNotifyCommand;
                }

                _notificationCache = value;

                if (_notificationCache != null)
                {
                    _notificationCache.ShouldNotifyProperty += NotificationCacheOnShouldNotifyProperty;
                    _notificationCache.ShouldNotifyCommand += NotificationCacheOnShouldNotifyCommand;
                }
            }
        }

        private void NotificationCacheOnShouldNotifyProperty(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        private void NotificationCacheOnShouldNotifyCommand(object sender, ShouldNotifyCommandEventArgs e)
        {
            RaiseCommandCanExecuteChanged(e.CommandNameToNotify);
        }

        private Dictionary<string, Tuple<PropertyInfo, IList<Attribute>>> _cachedAttributes;

        internal Dictionary<string, Tuple<PropertyInfo, IList<Attribute>>> CachedAttributes => _cachedAttributes ?? (_cachedAttributes = GetAllAttributes());
        
        private Dictionary<string, Tuple<PropertyInfo, IList<Attribute>>> GetAllAttributes()
        {
            var cachedAttributes = new Dictionary<string, Tuple<PropertyInfo, IList<Attribute>>>();
            
            foreach (var property in GetType().GetProperties().Where(p => p.GetMethod.IsPublic))
            {
                var customAttributes = property.GetCustomAttributes().ToList();
                cachedAttributes[property.Name] = new Tuple<PropertyInfo, IList<Attribute>>(property, customAttributes);
            }

            return cachedAttributes;
        }

        internal bool PropertyNameHasAttribute<TAttribute>(string propertyName) where TAttribute : Attribute
        {
            return CachedAttributes.TryGetValue(propertyName, out var propertyAttributes) && propertyAttributes.Item2.Any(a => a is TAttribute);
        }

        protected ComputedBindableBase()
        {
            NotificationCache = new NotificationCache(this);
        }
        
        /// <inheritdoc />
        protected override bool SetProperty<T>(ref T storage, T value, out T oldValue, [CallerMemberName] string propertyName = "")
        {
            var propertyWasChanged = base.SetProperty(ref storage, value, out oldValue, propertyName);
            if (propertyWasChanged)
            {
                if (oldValue is INotifyCollectionChanged oldCollection)
                    NotificationCache.UnregisterCollection(oldCollection, propertyName);

                if (storage is INotifyCollectionChanged newCollection)
                    NotificationCache.RegisterCollection(newCollection, propertyName);
            }
            return propertyWasChanged;
        }

        /// <inheritdoc />
        internal override void InternalRaisePropertyChanged(string propertyName)
        {
            base.InternalRaisePropertyChanged(propertyName);

            foreach (var propertyNameToNotify in _notificationCache.GetPropertyNamesToNotify(propertyName))
                base.InternalRaisePropertyChanged(propertyNameToNotify);

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
