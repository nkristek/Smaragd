using System;
using System.Collections.Generic;
using System.Linq;

namespace NKristek.Smaragd.Helpers
{
    /// <inheritdoc />
    internal sealed class NotificationCache
        : INotificationCache
    {
        private readonly Dictionary<string, IList<string>> _propertiesNotifyingProperties = new Dictionary<string, IList<string>>();

        private readonly Dictionary<string, IList<string>> _cachedPropertyNamesToNotify = new Dictionary<string, IList<string>>();

        private void InvalidateCache()
        {
            _cachedPropertyNamesToNotify.Clear();
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">If either <paramref name="propertyNameOfNotifyingProperty"/> or <paramref name="propertyNameToNotify"/> is null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="propertyNameOfNotifyingProperty"/> and <paramref name="propertyNameToNotify"/> are equal (a property should not notify itself).</exception>
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

        /// <inheritdoc />
        public IEnumerable<string> GetPropertyNamesToNotify(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
                return Enumerable.Empty<string>();

            if (_cachedPropertyNamesToNotify.TryGetValue(propertyName, out var cachedPropertyNamesToNotify))
                return cachedPropertyNamesToNotify.ToList();

            var propertyNamesToNotify = new List<string>();
            CalculateRecursivePropertyNamesToNotify(propertyName, propertyNamesToNotify);
            propertyNamesToNotify.Remove(propertyName);

            _cachedPropertyNamesToNotify[propertyName] = propertyNamesToNotify;
            return propertyNamesToNotify;
        }

        private void CalculateRecursivePropertyNamesToNotify(string propertyName, List<string> propertyNamesToNotify)
        {
            if (!_propertiesNotifyingProperties.TryGetValue(propertyName, out var propertyNamesToNotifyFromCurrentProperty))
                return;

            var newPropertyNames = propertyNamesToNotifyFromCurrentProperty.Where(name => !propertyNamesToNotify.Contains(name)).ToList();
            propertyNamesToNotify.AddRange(newPropertyNames);
            foreach (var propertyNameToNotify in newPropertyNames)
                CalculateRecursivePropertyNamesToNotify(propertyNameToNotify, propertyNamesToNotify);
        }
    }
}