using System.ComponentModel;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// Provides methods for raising events on the <see cref="INotifyPropertyChanging.PropertyChanging" /> event handler.
    /// </summary>
    public interface IRaisePropertyChanging
        : INotifyPropertyChanging
    {
        /// <summary>
        /// Raise an event on <see cref="INotifyPropertyChanging.PropertyChanging"/> to indicate that a property will change.
        /// </summary>
        /// <param name="propertyName">Name of the changing property.</param>
        void RaisePropertyChanging(string propertyName);
    }
}