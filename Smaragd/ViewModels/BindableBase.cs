using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NKristek.Smaragd.ViewModels
{
    /// <summary>
    /// Basic <see cref="INotifyPropertyChanged"/> implementation
    /// </summary>
    public abstract class BindableBase
        : INotifyPropertyChanged
    {
        /// <inheritdoc />
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise an event on <see cref="INotifyPropertyChanged.PropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">Name of the property which changed</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            InternalRaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Raise an event on <see cref="INotifyPropertyChanged.PropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">Name of the property which changed</param>
        internal virtual void InternalRaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Set the property value and raise an event on <see cref="INotifyPropertyChanged.PropertyChanged"/> if the value was different.
        /// </summary>
        /// <typeparam name="T">Type of the property to set</typeparam>
        /// <param name="storage">Reference to the storage variable</param>
        /// <param name="value">New value to set</param>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="oldValue">The old value of the property</param>
        /// <returns><c>True</c> if the value was different from the storage variable and the PropertyChanged event was raised</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, out T oldValue, [CallerMemberName] string propertyName = "")
        {
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            oldValue = storage;
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}
