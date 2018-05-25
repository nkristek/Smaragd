using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using NKristek.Smaragd.Attributes;

namespace NKristek.Smaragd.ViewModels
{
    /// <summary>
    /// This <see cref="ViewModel"/> implements <see cref="IDataErrorInfo"/> and <see cref="INotifyDataErrorInfo"/>. To set validation errors use <see cref="SetValidationError"/> or <see cref="SetValidationErrors"/> in the property setter of the property which should be validated. Additionally, in <see cref="Validate"/> all validation logic should be executed.
    /// </summary>
    public abstract class ValidatingViewModel
        : ViewModel, IDataErrorInfo, INotifyDataErrorInfo
    {
        public ValidatingViewModel()
        {
            ErrorsChanged += (sender, args) =>
            {
                RaisePropertyChanged(nameof(HasErrors));
            };
            
            ((INotifyCollectionChanged) Children).CollectionChanged += (sender, args) =>
            {
                if (args.OldItems != null)
                    foreach (var oldItem in args.OldItems.OfType<ValidatingViewModel>())
                        oldItem.ErrorsChanged -= OldItemOnErrorsChanged;

                if (args.NewItems != null)
                    foreach (var newItem in args.NewItems.OfType<ValidatingViewModel>())
                        newItem.ErrorsChanged += OldItemOnErrorsChanged;
            };
        }

        private void OldItemOnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(HasErrors));
        }

        private readonly object _lockObject = new object();

        private bool _validationSuspended;
        /// <summary>
        /// If the validation is temporarily suspended. Dispose the <see cref="IDisposable"/> from <see cref="SuspendValidation"/> to unsuspend. Setting this property will propagate the value to all <see cref="ValidatingViewModel"/> items in the <see cref="ViewModel.Children"/> collection.
        /// </summary>
        public bool ValidationSuspended
        {
            get
            {
                lock (_lockObject)
                {
                    return _validationSuspended;
                }
            }

            internal set
            {
                lock (_lockObject)
                {
                    _validationSuspended = value;
                    foreach (var validatingChild in Children.OfType<ValidatingViewModel>())
                        validatingChild.ValidationSuspended = value;
                }
            }
        }

        /// <summary>
        /// If data in this <see cref="ViewModel"/> is valid
        /// </summary>
        [IsDirtyIgnored]
        [PropertySource(nameof(HasErrors))]
        public bool IsValid => !HasErrors;

        private readonly Dictionary<string, List<string>> _validationErrors = new Dictionary<string, List<string>>();

        /// <summary>
        /// Set the validation error of the property
        /// </summary>
        /// <param name="error">This is the validation error and has to be set to null if no validation error occured</param>
        /// <param name="propertyName">Name of the property which validates with this error</param>
        protected void SetValidationError(string error, [CallerMemberName] string propertyName = null)
        {
            SetValidationErrors(Enumerable.Repeat(error, 1), propertyName);
        }

        /// <summary>
        /// Set multiple validation errors of the property
        /// </summary>
        /// <param name="errors">These are the validation errors and has to be empty if no validation error occured</param>
        /// <param name="propertyName">Name of the property which validates with this error</param>
        protected void SetValidationErrors(IEnumerable<string> errors, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                return;

            var errorList = errors.ToList();
            if (errorList.Any(e => !String.IsNullOrEmpty(e)))
                _validationErrors[propertyName] = errorList;
            else
                _validationErrors.Remove(propertyName);
            
            RaiseErrorsChanged(propertyName);
        }

        /// <summary>
        /// Raises an event on the ErrorsChanged event
        /// </summary>
        protected void RaiseErrorsChanged([CallerMemberName] string propertyName = null)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Concatenated validation errors
        /// </summary>
        public string Error
        {
            get { return String.Join(", ", _validationErrors.Select(kvp => $"{ kvp.Key }: { String.Join(", ", kvp.Value) }")); }
        }

        /// <summary>
        /// Get validation error by the name of the property
        /// </summary>
        /// <param name="columnName">Name of the property</param>
        /// <returns></returns>
        public string this[string columnName] => _validationErrors.ContainsKey(columnName) ? String.Join("\n", _validationErrors[columnName]) : String.Empty;

        /// <summary>
        /// Event that gets fired when the validation errors change
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Gets validation errors from a specified property
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <returns>Validation errors from the property</returns>
        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            return _validationErrors.ContainsKey(propertyName) ? _validationErrors[propertyName] : null;
        }

        /// <summary>
        /// Returns if there are any validation errors
        /// </summary>
        [IsDirtyIgnored]
        public bool HasErrors => _validationErrors.Any() || Children.OfType<ValidatingViewModel>().Any(c => c.HasErrors);

        /// <summary>
        /// All validation logic will be executed, even when <see cref="ValidationSuspended"/> is set to true.
        /// </summary>
        public void Validate()
        {
            var type = GetType();
            foreach (var propertyValidation in _validations)
            {
                var valueProperty = type.GetProperty(propertyValidation.Key);
                if (valueProperty == null)
                    continue;

                var value = valueProperty.GetValue(this, null);
                Validate(propertyValidation.Key, value, propertyValidation.Value);
            }

            foreach (var validatingChild in Children.OfType<ValidatingViewModel>())
                validatingChild.Validate();
        }

        private readonly Dictionary<string, IList<IValidation>> _validations = new Dictionary<string, IList<IValidation>>();

        /// <summary>
        /// Add a validation for the named property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="validation">Validation to add</param>
        /// <param name="initialValue">Initial value of the property for the initial run of the validation</param>
        protected void AddValidation<T>(string propertyName, Validation<T> validation, T initialValue = default)
        {
            if (_validations.TryGetValue(propertyName, out var existingValidations))
                existingValidations.Add(validation);
            else
                _validations.Add(propertyName, new List<IValidation> { validation });

            if (_validations.TryGetValue(propertyName, out var validations))
                Validate(propertyName, initialValue, validations.OfType<Validation<T>>());
        }

        /// <summary>
        /// Add a validation for the property returned by the lambda expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertySelector">Lambda expression to select the property. eg.: () => MyProperty</param>
        /// <param name="validation">Validation to add</param>
        protected void AddValidation<T>(Expression<Func<T>> propertySelector, Validation<T> validation)
        {
            AddValidation(GetPropertyName(propertySelector), validation, propertySelector.Compile()());
        }

        /// <summary>
        /// Removes a specific validation for the property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="validation">Validation to remove</param>
        /// <returns></returns>
        protected bool RemoveValidation<T>(string propertyName, Validation<T> validation)
        {
            if (!_validations.TryGetValue(propertyName, out var validations))
                return false;

            var validationWasRemoved = validations.Remove(validation);
            if (!validations.Any())
                _validations.Remove(propertyName);
            return validationWasRemoved;
        }

        /// <summary>
        /// Removes a specific validation for the property returned by the lambda expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertySelector">Lambda expression to select the property. eg.: () => MyProperty</param>
        /// <param name="validation">Validation to remove</param>
        /// <returns></returns>
        protected bool RemoveValidation<T>(Expression<Func<T>> propertySelector, Validation<T> validation)
        {
            return RemoveValidation(GetPropertyName(propertySelector), validation);
        }

        /// <summary>
        /// Removes all validations for the property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool RemoveValidations(string propertyName)
        {
            return _validations.Remove(propertyName);
        }

        /// <summary>
        /// Removes all validations for the property returned by the lambda expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertySelector">Lambda expression to select the property. eg.: () => MyProperty</param>
        /// <returns></returns>
        protected bool RemoveValidations<T>(Expression<Func<T>> propertySelector)
        {
            return RemoveValidations(GetPropertyName(propertySelector));
        }

        /// <summary>
        /// Get all validations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, IList<IValidation>>> Validations()
        {
            return _validations.ToList();
        }

        /// <summary>
        /// Get all validations for the property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IEnumerable<Validation<T>> Validations<T>(string propertyName)
        {
            return _validations.ContainsKey(propertyName)
                ? _validations[propertyName].OfType<Validation<T>>()
                : Enumerable.Empty<Validation<T>>();
        }

        /// <summary>
        /// Get all validations for the property returned by the lambda expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertySelector">Lambda expression to select the property. eg.: () => MyProperty</param>
        /// <returns></returns>
        public IEnumerable<Validation<T>> Validations<T>(Expression<Func<T>> propertySelector)
        {
            return Validations<T>(GetPropertyName(propertySelector));
        }

        private static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new Exception("Expression body is not of type MemberExpression");

            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Sets a property if <see cref="ViewModel.IsReadOnly"/> is not true and the value is different and raises an event on the <see cref="PropertyChangedEventHandler"/>.
        /// It will execute the appropriate validations for this property.
        /// </summary>
        /// <typeparam name="T">Type of the property to set</typeparam>
        /// <param name="storage">Reference to the storage variable</param>
        /// <param name="value">New value to set</param>
        /// <param name="propertyName">Name of the property</param>
        /// <returns>True if the value was different from the storage variable and the PropertyChanged event was raised</returns>
        protected override bool SetProperty<T>(ref T storage, T value, out T oldValue, [CallerMemberName] string propertyName = "")
        {
            var propertyWasChanged = base.SetProperty(ref storage, value, out oldValue, propertyName);
            if (propertyWasChanged && _validations.TryGetValue(propertyName, out var validations))
                Validate(propertyName, value, validations.OfType<Validation<T>>());
            return propertyWasChanged;
        }

        private void Validate<T>(string propertyName, T value, IEnumerable<Validation<T>> validations)
        {
            var errors = new List<string>();
            foreach (var validation in validations)
            {
                if (!validation.IsValid(value, out var errorMessage))
                    errors.Add(errorMessage);
            }
            SetValidationErrors(errors, propertyName);
        }

        private void Validate(string propertyName, object value, IEnumerable<IValidation> validations)
        {
            var errors = new List<string>();
            foreach (var validation in validations)
            {
                if (!validation.IsValid(value, out var errorMessage))
                    errors.Add(errorMessage);
            }
            SetValidationErrors(errors, propertyName);
        }

        /// <summary>
        /// Temporarily suspends validation. This could be used in a batch update to prevent validation overhead. This will propagate to all <see cref="ValidatingViewModel"/> items in the <see cref="ViewModel.Children"/> collection.
        /// </summary>
        /// <returns><see cref="IDisposable"/> which unsuspends validation when disposed.</returns>
        public IDisposable SuspendValidation()
        {
            return new SuspendValidationDisposable(this);
        }
    }
}
