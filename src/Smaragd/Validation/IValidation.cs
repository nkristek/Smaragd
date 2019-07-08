namespace NKristek.Smaragd.Validation
{
    /// <summary>
    /// Validates a given value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to validate.</typeparam>
    /// <typeparam name="TValidationResult">The type of the result.</typeparam>
    public interface IValidation<in TValue, out TValidationResult>
    {
        /// <summary>
        /// Validates the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <returns>The result of the validation.</returns>
        TValidationResult Validate(TValue value);
    }
}