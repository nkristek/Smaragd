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

        internal HashSet<string> IsDirtyIgnoredProperties = new HashSet<string>();

        /// <inheritdoc />
        protected ComputedBindable()
        {
            InitAttributes();
        }

        private void InitAttributes()
        {
            var inheritPropertySource = new Dictionary<string, bool>();
            var inheritIsDirty = new Dictionary<string, bool>();

            var currentType = GetType();
            while (currentType != null)
            {
                var properties = currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (var property in properties)
                {
                    var attributes = property.GetCustomAttributes(false);

                    if (!inheritPropertySource.ContainsKey(property.Name) || inheritPropertySource[property.Name])
                    {
                        var propertySourceAttribute = attributes.OfType<PropertySourceAttribute>().SingleOrDefault();
                        if (propertySourceAttribute != null)
                        {
                            inheritPropertySource[property.Name] = propertySourceAttribute.InheritAttributes;

                            foreach (var propertySource in propertySourceAttribute.PropertySources)
                                _notificationCache.AddPropertyNameToNotify(propertySource, property.Name);
                        }
                        else
                        {
                            inheritPropertySource[property.Name] = false;
                        }
                    }

                    if (!inheritIsDirty.ContainsKey(property.Name) || inheritIsDirty[property.Name])
                    {
                        var isDirtyIgnoredAttribute = attributes.OfType<IsDirtyIgnoredAttribute>().SingleOrDefault();
                        if (isDirtyIgnoredAttribute != null)
                        {
                            if (isDirtyIgnoredAttribute.InheritAttributes)
                                inheritIsDirty[property.Name] = true;
                            else
                                IsDirtyIgnoredProperties.Add(property.Name);
                        }
                        else
                        {
                            inheritIsDirty[property.Name] = false;
                        }
                    }
                }
                currentType = currentType.BaseType;
            }
        }
        
        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">If <paramref name="propertyName"/> is null or whitespace.</exception>
        public sealed override void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            var additionalPropertyNames = _notificationCache.GetPropertyNamesToNotify(propertyName);
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

            base.RaisePropertyChanged(propertyName);

            if (additionalPropertyNames == null)
                return;

            foreach (var propertyNameToNotify in additionalPropertyNames)
                base.RaisePropertyChanged(propertyNameToNotify);
        }
    }
}