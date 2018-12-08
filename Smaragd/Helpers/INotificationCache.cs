using System.Collections.Generic;

namespace NKristek.Smaragd.Helpers
{
    /// <summary>
    /// Calculates which property names to notify from a property name.
    /// </summary>
    internal interface INotificationCache
    {
        /// <summary>
        /// Add a name of a property which should notify another property.
        /// </summary>
        /// <param name="propertyNameOfNotifyingProperty">The name of the property which is notifying the other property.</param>
        /// <param name="propertyNameToNotify">The name of the property which should be notified.</param>
        void AddPropertyNameToNotify(string propertyNameOfNotifyingProperty, string propertyNameToNotify);

        /// <summary>
        /// <para>
        /// Get the names of all properties which should be notified by this property name.
        /// </para>
        /// <para>
        /// This includes indirectly notified properties and excludes the name of the property itself, if it gets notified indirectly by a notified property (Prop1 -> Prop2 -> Prop1).
        /// </para>
        /// </summary>
        /// <param name="propertyName">The name of the property which notifies properties</param>
        /// <returns>The names of properties which get notified by <paramref name="propertyName"/> either directly or indirectly, excluding <paramref name="propertyName"/> itself.</returns>
        IEnumerable<string> GetPropertyNamesToNotify(string propertyName);
    }
}