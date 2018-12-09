using System.ComponentModel;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// Provides methods for raising events on the <see cref="INotifyPropertyChanged.PropertyChanged" /> event handler.
    /// </summary>
    public interface IRaisePropertyChanged
        : INotifyPropertyChanged
    {
        /// <summary>
        /// Raise an event on <see cref="INotifyPropertyChanged.PropertyChanged"/> to indicate that a property changed.
        /// </summary>
        /// <param name="propertyName">Name of the changed property.</param>
        void RaisePropertyChanged(string propertyName);
    }
}
