using System;
using System.Collections.Generic;
using System.Linq;

namespace NKristek.Smaragd.Helpers
{
    internal sealed class NotificationCache
        : INotificationCache
    {
        private readonly Dictionary<string, IList<string>> _cachedPropertyNamesToNotify = new Dictionary<string, IList<string>>();

        /// <summary>
        /// Key is property name of notifying property, value is list of all properties to notify
        /// </summary>
        private readonly Dictionary<string, IList<string>> _propertiesNotifyingProperties = new Dictionary<string, IList<string>>();
        
        /// <inheritdoc />
        public void AddPropertyNameToNotify(string propertyNameOfNotifyingProperty, string propertyNameToNotify)
        {
            if (String.IsNullOrEmpty(propertyNameOfNotifyingProperty))
                throw new ArgumentNullException(nameof(propertyNameOfNotifyingProperty));

            if (String.IsNullOrEmpty(propertyNameToNotify))
                throw new ArgumentNullException(nameof(propertyNameToNotify));

            if (propertyNameOfNotifyingProperty == propertyNameToNotify)
                throw new ArgumentException("The notifying property should not notify itself.");

            InvalidateCache();

            if (!_propertiesNotifyingProperties.ContainsKey(propertyNameOfNotifyingProperty))
                _propertiesNotifyingProperties[propertyNameOfNotifyingProperty] = new List<string>();

            if (!_propertiesNotifyingProperties[propertyNameOfNotifyingProperty].Contains(propertyNameToNotify))
                _propertiesNotifyingProperties[propertyNameOfNotifyingProperty].Add(propertyNameToNotify);
        }

        private void InvalidateCache()
        {
            _cachedPropertyNamesToNotify.Clear();
        }

        /// <inheritdoc />
        public IEnumerable<string> GetPropertyNamesToNotify(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            if (_cachedPropertyNamesToNotify.TryGetValue(propertyName, out var cachedPropertyNamesToNotify))
                return cachedPropertyNamesToNotify.ToList();

            _cachedPropertyNamesToNotify[propertyName] = GetRecursivePropertyNamesToNotify(propertyName, new List<string>()).ToList();
            _cachedPropertyNamesToNotify[propertyName].Remove(propertyName); // a property should not notify itself
            return _cachedPropertyNamesToNotify[propertyName].ToList(); // return a new list so the internal one can not be modified
        }

        private IEnumerable<string> GetRecursivePropertyNamesToNotify(string propertyName, IList<string> propertyNamesToNotify)
        {
            if (!_propertiesNotifyingProperties.TryGetValue(propertyName, out var currentPropertyNamesToNotify))
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
    }
}
