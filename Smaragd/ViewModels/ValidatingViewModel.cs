using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.Validation;

namespace NKristek.Smaragd.ViewModels
{
    /// <summary>
    /// This <see cref="ViewModel"/> implements <see cref="IDataErrorInfo"/> and <see cref="INotifyDataErrorInfo"/>
    /// </summary>
    public abstract class ValidatingViewModel
        : ViewModel, IDataErrorInfo, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, IList<IValidation>> _validations = new Dictionary<string, IList<IValidation>>();

        private readonly Dictionary<string, IList<string>> _validationErrors = new Dictionary<string, IList<string>>();

        /// <inheritdoc />
        protected ValidatingViewModel()
        {
            ((INotifyCollectionChanged) Children).CollectionChanged += OnChildrenCollectionChanged;
        }

        private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Move)
                RaiseErrorsChanged(nameof(Children));
        }

        #region IDataErrorInfo
        
        /// <inheritdoc />
        public string this[string propertyName]
        {
            get
            {
                if (String.IsNullOrEmpty(propertyName))
                    return Error;

                if (_validationErrors.ContainsKey(propertyName))
                    return String.Join(Environment.NewLine, _validationErrors[propertyName]);

                if (nameof(Children).Equals(propertyName))
                    return String.Join(Environment.NewLine, GetChildrenErrors());

                return String.Empty;
            }
        }

        /// <inheritdoc />
        public string Error => String.Join(Environment.NewLine, GetAllErrors());

        private IEnumerable<string> GetAllErrors()
        {
            var errors = _validationErrors.SelectMany(kvp => kvp.Value).Where(e => !String.IsNullOrEmpty(e));
            var childrenErrors = GetChildrenErrors();
            return errors.Concat(childrenErrors);
        }

        private IEnumerable<string> GetChildrenErrors()
        {
            return Children.OfType<ValidatingViewModel>().SelectMany(c => c.GetErrors(null).OfType<string>()).Where(e => !String.IsNullOrEmpty(e));
        }

        #endregion

        #region INotifyDataErrorInfo

        /// <inheritdoc />
        [IsDirtyIgnored]
        [PropertySourceCollection(nameof(Children))]
        public bool HasErrors => _validationErrors.Any() || Children.OfType<ValidatingViewModel>().Any(c => c.HasErrors);

        /// <inheritdoc />
        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
                return GetAllErrors();

            if (_validationErrors.ContainsKey(propertyName))
                return _validationErrors[propertyName];

            if (nameof(Children).Equals(propertyName))
                return GetChildrenErrors();

            return Enumerable.Empty<string>();
        }

        /// <inheritdoc />
        public virtual event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Raises an event on the <see cref="ErrorsChanged"/> event
        /// </summary>
        protected internal void RaiseErrorsChanged(string propertyName = null)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            RaisePropertyChanged(nameof(HasErrors));
            (Parent as ValidatingViewModel)?.RaiseErrorsChanged(nameof(Children));
        }

        #endregion

        private bool _validationSuspended;

        /// <summary>
        /// If the validation is temporarily suspended. Dispose the <see cref="IDisposable"/> from <see cref="SuspendValidation"/> to unsuspend. Setting this property will propagate the value to all <see cref="ValidatingViewModel"/> items in the <see cref="ViewModel.Children"/> collection.
        /// </summary>
        public bool ValidationSuspended
        {
            get => _validationSuspended;
            internal set
            {
                if (SetProperty(ref _validationSuspended, value, out _))
                {
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
        
        private void SetValidationErrors(string propertyName, IEnumerable<string> errors)
        {
            if (propertyName == null)
                return;

            var errorList = errors.Where(e => !String.IsNullOrEmpty(e)).ToList();
            if (errorList.Any())
            {
                _validationErrors[propertyName] = errorList;
                RaiseErrorsChanged(propertyName);
            }
            else if(_validationErrors.Remove(propertyName))
            {
                RaiseErrorsChanged(propertyName);
            }

        }
        
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
        
        /// <summary>
        /// Add a validation for the property returned by the lambda expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertySelector">Lambda expression to select the property. eg.: () => MyProperty</param>
        /// <param name="validation">Validation to add</param>
        protected void AddValidation<T>(Expression<Func<T>> propertySelector, Validation<T> validation)
        {
            var propertyName = GetPropertyName(propertySelector);
            var initialValue = propertySelector.Compile()();

            if (_validations.TryGetValue(propertyName, out var existingValidations))
            {
                existingValidations.Add(validation);
            }
            else
            {
                existingValidations = new List<IValidation> { validation };
                _validations.Add(propertyName, existingValidations);
            }

            Validate(propertyName, initialValue, existingValidations.OfType<Validation<T>>());
        }
        
        /// <summary>
        /// Removes a specific validation for the property returned by the expression
        /// </summary>
        /// <typeparam name="T">Type of the property to validate</typeparam>
        /// <param name="propertySelector">Expression to select the property. eg.: () => MyProperty</param>
        /// <param name="validation">Validation to remove</param>
        /// <returns>If the validation was found and successfully removed</returns>
        protected bool RemoveValidation<T>(Expression<Func<T>> propertySelector, Validation<T> validation)
        {
            var propertyName = GetPropertyName(propertySelector);
            if (!_validations.TryGetValue(propertyName, out var validations))
                return false;

            var validationWasRemoved = validations.Remove(validation);
            if (!validations.Any())
                _validations.Remove(propertyName);
            return validationWasRemoved;
        }

        /// <summary>
        /// Removes all validations for the property returned by the expression
        /// </summary>
        /// <typeparam name="T">Type of the validating property</typeparam>
        /// <param name="propertySelector">Expression to select the property. eg.: () => MyProperty</param>
        /// <returns>If the validation was found and successfully removed</returns>
        protected bool RemoveValidations<T>(Expression<Func<T>> propertySelector)
        {
            var propertyName = GetPropertyName(propertySelector);
            return _validations.Remove(propertyName);
        }

        /// <summary>
        /// Get all validations. Key is the name of the property, value are all validations for the property.
        /// </summary>
        /// <returns>All validations. Key is the name of the property, value are all validations for the property.</returns>
        public IEnumerable<KeyValuePair<string, IList<IValidation>>> Validations()
        {
            return _validations.ToList();
        }

        /// <summary>
        /// Get all validations for the property returned by the expression
        /// </summary>
        /// <typeparam name="T">Type of the validating property</typeparam>
        /// <param name="propertySelector">Expression to select the property. eg.: () => MyProperty</param>
        /// <returns>All validations for the property</returns>
        public IEnumerable<Validation<T>> Validations<T>(Expression<Func<T>> propertySelector)
        {
            var propertyName = GetPropertyName(propertySelector);
            return _validations.ContainsKey(propertyName)
                ? _validations[propertyName].OfType<Validation<T>>()
                : Enumerable.Empty<Validation<T>>();
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
        /// <param name="oldValue">The old value of the property</param>
        /// <param name="propertyName">Name of the property</param>
        /// <returns>True if the value was different from the storage variable and the PropertyChanged event was raised</returns>
        protected override bool SetProperty<T>(ref T storage, T value, out T oldValue, [CallerMemberName] string propertyName = "")
        {
            var propertyWasChanged = base.SetProperty(ref storage, value, out oldValue, propertyName);
            if (propertyWasChanged && !ValidationSuspended && _validations.TryGetValue(propertyName, out var validations))
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
            SetValidationErrors(propertyName, errors);
        }
        
        private void Validate(string propertyName, object value, IEnumerable<IValidation> validations)
        {
            var errors = new List<string>();
            foreach (var validation in validations)
            {
                if (!validation.IsValid(value, out var errorMessage))
                    errors.Add(errorMessage);
            }
            SetValidationErrors(propertyName, errors);
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
