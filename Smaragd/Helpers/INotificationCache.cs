using System.Collections.Generic;

namespace NKristek.Smaragd.Helpers
{
    internal interface INotificationCache
    {
        /// <summary>
        /// Add a name of a property to this <see cref="INotificationCache"/>
        /// </summary>
        /// <param name="propertyNameOfNotifyingProperty">The name of the property which is notifying the other property</param>
        /// <param name="propertyNameToNotify">The name of the property which should be notified</param>
        void AddPropertyNameToNotify(string propertyNameOfNotifyingProperty, string propertyNameToNotify);

        /// <summary>
        /// Get all names of properties which should be notified by this property name. This includes properties which get notified by properties which get notified by this property.
        /// <para/>
        /// Example: If <paramref name="propertyName"/> notifies another property and this property notifies a third property, then the name of the third property will also be returned
        /// </summary>
        /// <param name="propertyName">The name of the property which notifies properties</param>
        /// <returns></returns>
        IEnumerable<string> GetPropertyNamesToNotify(string propertyName);
    }
}
