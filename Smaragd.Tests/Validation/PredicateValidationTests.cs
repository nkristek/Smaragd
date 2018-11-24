using Xunit;
using NKristek.Smaragd.Validation;

namespace NKristek.Smaragd.Tests.Validation
{
    public class PredicateValidationTests
    {
        [Fact]
        public void TestIsValid()
        {
            var errorMessage = "Value should be at least 5.";
            var validation = new PredicateValidation<int>(i => i >= 5, errorMessage);

            Assert.False(validation.IsValid(4, out var returnedErrorMessage), "IsValid did not return false.");
            Assert.Equal(errorMessage, returnedErrorMessage);

            Assert.True(validation.IsValid(5, out returnedErrorMessage), "IsValid did not return true.");
            Assert.Null(returnedErrorMessage);
        }
    }
}
