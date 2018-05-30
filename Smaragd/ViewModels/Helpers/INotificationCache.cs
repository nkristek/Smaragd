using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace NKristek.Smaragd.ViewModels.Helpers
{
    internal interface INotificationCache
        : INotifyPropertyChanged
    {
        /// <summary>
        /// This event is raised when a command should be notified
        /// </summary>
        event ShouldNotifyCommandEventHandler ShouldNotifyCommand;

        /// <summary>
        /// Add a name of a property to this <see cref="INotificationCache"/>
        /// </summary>
        /// <param name="propertyNameOfNotifyingProperty">The name of the property which is notifying the other property</param>
        /// <param name="propertyNameToNotify">The name of the property which should be notified</param>
        void AddPropertyNameToNotify(string propertyNameOfNotifyingProperty, string propertyNameToNotify);

        /// <summary>
        /// Add a name of a command property to this <see cref="INotificationCache"/>.
        /// </summary>
        /// <param name="propertyNameOfNotifyingProperty">The name of the property which is notifying the other property</param>
        /// <param name="commandNameToNotify">The name of the property which should be notified</param>
        void AddCommandNameToNotify(string propertyNameOfNotifyingProperty, string commandNameToNotify);

        /// <summary>
        /// Get all names of properties which should be notified by this property name
        /// </summary>
        /// <param name="propertyName">The name of the property which notifies properties</param>
        /// <returns></returns>
        IEnumerable<string> GetPropertyNamesToNotify(string propertyName);

        /// <summary>
        /// Get all names of command properties which should be notified by this property name
        /// </summary>
        /// <param name="propertyName">The name of the property which notifies commands</param>
        /// <returns></returns>
        IEnumerable<string> GetCommandNamesToNotify(string propertyName);

        void AddPropertyNameToNotify(string propertyNameOfNotifyingCollection, IEnumerable<NotifyCollectionChangedAction> collectionActions, string propertyNameToNotify);
        void AddCommandNameToNotify(string propertyNameOfNotifyingCollection, IEnumerable<NotifyCollectionChangedAction> collectionActions, string commandNameToNotify);

        /// <summary>
        /// Register a collection for the specified name. When <see cref="INotifyCollectionChanged.CollectionChanged"/> events are raised properties and commands are notified which got added to this <see cref="INotificationCache"/>.
        /// </summary>
        /// <param name="collection">Collection to add</param>
        /// <param name="propertyName">Property name of the collection</param>
        void RegisterCollection(INotifyCollectionChanged collection, string propertyName);

        /// <summary>
        /// Unregisters a collection. This <see cref="INotificationCache"/> will no longer listen for <see cref="INotifyCollectionChanged.CollectionChanged"/> events.
        /// </summary>
        /// <param name="collection">Collection to remove</param>
        /// <param name="propertyName">Property name of the collection</param>
        void UnregisterCollection(INotifyCollectionChanged collection, string propertyName);
    }
}
