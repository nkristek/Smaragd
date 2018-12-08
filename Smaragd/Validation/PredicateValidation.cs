using System;

namespace NKristek.Smaragd.Validation
{
    /// <inheritdoc />
    /// <remarks>
    /// Validates a given value using a predicate.
    /// </remarks>
    public class PredicateValidation<T>
        : Validation<T>
    {
        private readonly Predicate<T> _predicate;

        private readonly string _errorMessage;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="PredicateValidation{T}"/> class with the <paramref name="predicate"/> which is used to validate the value and the <paramref name="errorMessage"/> which will be returned when the <paramref name="predicate"/> returns <c>false</c> for a given value.
        /// </summary>
        /// <param name="predicate">A <see cref="Predicate{T}"/> which is used to validate the value.</param>
        /// <param name="errorMessage">The error message which will be returned when the <paramref name="predicate"/> returns <c>false</c> for a given value.</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="predicate"/> or <paramref name="errorMessage"/> is null.</exception>
        public PredicateValidation(Predicate<T> predicate, string errorMessage)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _errorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
        }

        /// <inheritdoc />
        public override bool IsValid(T value, out string errorMessage)
        {
            errorMessage = null;
            if (_predicate(value))
                return true;
            errorMessage = _errorMessage;
            return false;
        }
    }
}