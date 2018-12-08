using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.Helpers;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc />
    /// <remarks>
    /// This class adds support for using the <see cref="PropertySourceAttribute" /> above properties.
    /// </remarks>
    public abstract class ComputedBindable
        : Bindable
    {
        private readonly INotificationCache _notificationCache = new NotificationCache();

        /// <inheritdoc />
        protected ComputedBindable()
        {
            InitializeNotificationCache();
        }

        private void InitializeNotificationCache()
        {
            var allPropertyNames = GetType().GetProperties().Where(p => p.GetMethod.IsPublic).Select(p => p.Name).ToList();
            foreach (var propertyAttributes in CachedAttributes)
            {
                var propertyName = propertyAttributes.Key;
                var attributes = propertyAttributes.Value;
                foreach (var attribute in attributes.OfType<PropertySourceAttribute>().Where(a => a.PropertySources != null))
                foreach (var propertySource in attribute.PropertySources.Where(ps => ps != propertyName && allPropertyNames.Contains(ps)))
                    _notificationCache.AddPropertyNameToNotify(propertySource, propertyName);
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">If <paramref name="propertyName"/> is null or whitespace.</exception>
        protected sealed override void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            var additionalPropertyNames = _notificationCache.GetPropertyNamesToNotify(propertyName).ToList();
            RaisePropertyChanged(propertyName, additionalPropertyNames);
        }

        /// <summary>
        /// Raises events on <see cref="INotifyPropertyChanged.PropertyChanged"/> for the <paramref name="propertyName"/> and the <paramref name="additionalPropertyNames"/>, which are propertyNames which get notified because of the <see cref="PropertySourceAttribute"/>.
        /// </summary>
        /// <param name="propertyName">Name of the changed property.</param>
        /// <param name="additionalPropertyNames">Names of properties which get notified because of the <see cref="PropertySourceAttribute"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="propertyName"/> is null or whitespace.</exception>
        protected virtual void RaisePropertyChanged(string propertyName, IEnumerable<string> additionalPropertyNames)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            var propertyNamesToNotify = new List<string> {propertyName};
            if (additionalPropertyNames != null)
                propertyNamesToNotify.AddRange(additionalPropertyNames);

            foreach (var propertyNameToNotify in propertyNamesToNotify)
                base.RaisePropertyChanged(propertyNameToNotify);
        }

        private Dictionary<string, IList<Attribute>> _cachedAttributes;

        private Dictionary<string, IList<Attribute>> CachedAttributes => _cachedAttributes ?? (_cachedAttributes = GetAllAttributes());

        private Dictionary<string, IList<Attribute>> GetAllAttributes()
        {
            var cachedAttributes = new Dictionary<string, IList<Attribute>>();

            foreach (var property in GetType().GetProperties().Where(p => p.GetMethod.IsPublic))
                cachedAttributes[property.Name] = property.GetCustomAttributes().ToList();

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
            return CachedAttributes.TryGetValue(propertyName, out var propertyAttributes) && propertyAttributes.Any(a => a is TAttribute);
        }
    }
}