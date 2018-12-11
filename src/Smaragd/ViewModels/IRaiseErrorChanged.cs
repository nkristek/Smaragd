using System.ComponentModel;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// Provides methods for raising events on the <see cref="INotifyDataErrorInfo.ErrorsChanged" /> event handler.
    /// </summary>
    public interface IRaiseErrorsChanged
        : INotifyDataErrorInfo
    {
        /// <summary>
        /// Raises an event on the <see cref="INotifyDataErrorInfo.ErrorsChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Property name of which the validation errors changed.</param>
        void RaiseErrorsChanged(string propertyName = null);
    }
}