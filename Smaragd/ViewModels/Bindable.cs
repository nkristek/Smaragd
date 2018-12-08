using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// This class provides an implementation of <see cref="INotifyPropertyChanged"/> and methods to set the value of a property and automatically raise an event on <see cref="PropertyChanged"/> if the value changed.
    /// </summary>
    public abstract class Bindable
        : INotifyPropertyChanged
    {
        /// <inheritdoc />
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise an event on <see cref="INotifyPropertyChanged.PropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">Name of the changed property.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="propertyName"/> is null or whitespace.</exception>
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Set the property value and raise an event on <see cref="INotifyPropertyChanged.PropertyChanged"/> if the value changed.
        /// </summary>
        /// <typeparam name="T">Type of the property to set.</typeparam>
        /// <param name="storage">Reference to the storage variable.</param>
        /// <param name="value">New value to set.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value of the property.</param>
        /// <returns><c>True</c> if the value was different from the storage variable and the PropertyChanged event was raised</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="propertyName"/> is null or whitespace.</exception>
        protected virtual bool SetProperty<T>(ref T storage, T value, out T oldValue, [CallerMemberName] string propertyName = "")
        {
            if (String.IsNullOrWhiteSpace(propertyName))
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