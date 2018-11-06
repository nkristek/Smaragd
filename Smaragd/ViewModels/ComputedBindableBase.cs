using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.Helpers;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// This class adds support to use the <see cref="PropertySourceAttribute" /> above properties and <see cref="CanExecuteSourceAttribute" /> above <see cref="ICommand.CanExecute"/> in commands implementing <see cref="IRaiseCanExecuteChanged" />.
    /// </summary>
    public abstract class ComputedBindableBase
        : BindableBase
    {
        private readonly INotificationCache _notificationCache = new NotificationCache();

        /// <inheritdoc />
        protected ComputedBindableBase()
        {
            var propertyNames = GetType().GetProperties().Where(p => p.GetMethod.IsPublic).Select(p => p.Name).ToList();
            foreach (var propertyAttributes in CachedAttributes)
            {
                var propertyName = propertyAttributes.Key;
                var attributes = propertyAttributes.Value.Item2;

                foreach (var attribute in attributes.OfType<PropertySourceAttribute>().Where(a => a.PropertySources != null))
                {
                    // filter propertysources where the source is the name of the property itself or doesnt exist
                    foreach (var propertySource in attribute.PropertySources.Where(ps => ps != propertyName && propertyNames.Contains(ps)))
                        _notificationCache.AddPropertyNameToNotify(propertySource, propertyName);
                }
            }
        }
        
        /// <inheritdoc />
        internal override void InternalRaisePropertyChanged(string propertyName)
        {
            base.InternalRaisePropertyChanged(propertyName);

            // notify properties
            var propertyNamesToNotify = _notificationCache.GetPropertyNamesToNotify(propertyName).ToList();
            foreach (var propertyNameToNotify in propertyNamesToNotify)
                base.InternalRaisePropertyChanged(propertyNameToNotify);

            // notify commands
            propertyNamesToNotify.Insert(0, propertyName);
            try
            {
                foreach (var commandProperty in GetType().GetProperties().Where(p => p.GetMethod.IsPublic && typeof(IRaiseCanExecuteChanged).IsAssignableFrom(p.PropertyType)))
                {
                    try
                    {
                        if (commandProperty.GetValue(this, null) is IRaiseCanExecuteChanged command &&
                            command.ShouldRaiseCanExecuteChanged(propertyNamesToNotify))
                            command.RaiseCanExecuteChanged();
                    }
                    catch { }
                }
            }
            catch { }
        }

        #region Cached Attributes

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

        /// <summary>
        /// Returns if the attribute is set on the property with the given name
        /// </summary>
        /// <typeparam name="TAttribute">The <see cref="Attribute"/> which may exist on the property</typeparam>
        /// <param name="propertyName">Name of the property</param>
        /// <returns>If the attribute is set on the property</returns>
        internal bool PropertyNameHasAttribute<TAttribute>(string propertyName) where TAttribute : Attribute
        {
            return CachedAttributes.TryGetValue(propertyName, out var propertyAttributes) && propertyAttributes.Item2.Any(a => a is TAttribute);
        }

        #endregion
    }
}
