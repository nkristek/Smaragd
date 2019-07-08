using System.Collections.Generic;
using System.ComponentModel;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc cref="IViewModel" />
    /// <summary>
    /// Defines a <see cref="IViewModel"/> which performs validation.
    /// </summary>
    public interface IValidatingViewModel
        : IViewModel, INotifyDataErrorInfo
    {
        /// <summary>
        /// If data is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Set validation errors of a property.
        /// </summary>
        /// <param name="errors">The errors of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        void SetErrors(IEnumerable<string> errors, string propertyName = null);
    }
}