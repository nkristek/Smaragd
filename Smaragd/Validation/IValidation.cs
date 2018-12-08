namespace NKristek.Smaragd.Validation
{
    /// <summary>
    /// Validates a given value.
    /// </summary>
    public interface IValidation
    {
        /// <summary>
        /// Validates the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        /// <param name="errorMessage">Error message when the value is not valid.</param>
        /// <returns>If the given <paramref name="value"/> is valid.</returns>
        bool IsValid(object value, out string errorMessage);
    }
}