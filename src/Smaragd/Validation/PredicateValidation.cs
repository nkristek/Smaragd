using System;

namespace NKristek.Smaragd.Validation
{
    /// <inheritdoc />
    /// <remarks>
    /// Validates a given value using a predicate.
    /// </remarks>
    public sealed class PredicateValidation<TValue>
        : IValidation<TValue, bool>
    {
        private readonly Predicate<TValue> _validationPredicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="PredicateValidation{TValue}"/> class with the <paramref name="validationPredicate"/> which is used to validate the value.
        /// </summary>
        /// <param name="validationPredicate">A <see cref="Predicate{T}"/> which is used to validate the value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="validationPredicate"/> is <see langword="null"/>.</exception>
        public PredicateValidation(Predicate<TValue> validationPredicate)
        {
            _validationPredicate = validationPredicate ?? throw new ArgumentNullException(nameof(validationPredicate));
        }

        /// <inheritdoc />
        public bool Validate(TValue value)
        {
            return _validationPredicate(value);
        }
    }
}
