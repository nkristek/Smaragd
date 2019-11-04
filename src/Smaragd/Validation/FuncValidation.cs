using System;

namespace NKristek.Smaragd.Validation
{
    /// <inheritdoc />
    /// <remarks>
    /// Validates a given value using a function.
    /// </remarks>
    public sealed class FuncValidation<TValue, TResult>
        : IValidation<TValue, TResult>
    {
        private readonly Func<TValue, TResult> _validationFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncValidation{TValue, TValidationResult}"/> class with the <paramref name="validationFunc"/> which is used to validate the value.
        /// </summary>
        /// <param name="validationFunc">A <see cref="Func{T, TResult}"/> which is used to validate the value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="validationFunc"/> is <see langword="null"/>.</exception>
        public FuncValidation(Func<TValue, TResult> validationFunc)
        {
            _validationFunc = validationFunc ?? throw new ArgumentNullException(nameof(validationFunc));
        }

        /// <inheritdoc />
        public TResult Validate(TValue value)
        {
            return _validationFunc(value);
        }
    }
}
