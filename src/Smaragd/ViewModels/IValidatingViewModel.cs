using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using NKristek.Smaragd.Validation;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc cref="IViewModel" />
    /// <summary>
    /// Defines a <see cref="IViewModel"/> which performs validation.
    /// </summary>
    public interface IValidatingViewModel
        : IViewModel, IDataErrorInfo, IRaiseErrorsChanged
    {
        /// <summary>
        /// If data is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// If validation is temporarily suspended.
        /// </summary>
        bool IsValidationSuspended { get; set; }

        /// <summary>
        /// Add a validation for the property returned by the lambda expression.
        /// </summary>
        /// <typeparam name="T">The type of the property to validate.</typeparam>
        /// <param name="propertySelector">An expression to select the property. eg.: () => MyProperty</param>
        /// <param name="validation">The validation to add.</param>
        void AddValidation<T>(Expression<Func<T>> propertySelector, IValidation<T> validation);

        /// <summary>
        /// Removes a specific validation of the property returned by the expression.
        /// </summary>
        /// <typeparam name="T">The type of the property to validate.</typeparam>
        /// <param name="propertySelector">An expression to select the property. eg.: () => MyProperty</param>
        /// <param name="validation">The validation to remove.</param>
        /// <returns>If the validation was found and successfully removed</returns>
        bool RemoveValidation<T>(Expression<Func<T>> propertySelector, IValidation<T> validation);

        /// <summary>
        /// Removes all validations of the property returned by the expression.
        /// </summary>
        /// <typeparam name="T">The type of the validating property.</typeparam>
        /// <param name="propertySelector">An expression to select the property. eg.: () => MyProperty</param>
        /// <returns>If the validation was found and successfully removed.</returns>
        bool RemoveValidations<T>(Expression<Func<T>> propertySelector);

        /// <summary>
        /// Get all validations.
        /// </summary>
        /// <returns>All validations. Key is the name of the property, value are all validations for the property.</returns>
        IEnumerable<KeyValuePair<string, IList<IValidation>>> Validations();

        /// <summary>
        /// Get all validations of the property returned by the expression.
        /// </summary>
        /// <typeparam name="T">The type of the validating property.</typeparam>
        /// <param name="propertySelector">An expression to select the property. eg.: () => MyProperty</param>
        /// <returns>All validations of the property.</returns>
        IEnumerable<IValidation<T>> Validations<T>(Expression<Func<T>> propertySelector);
    }
}