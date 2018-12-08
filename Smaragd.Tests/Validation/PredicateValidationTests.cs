using System;
using NKristek.Smaragd.Validation;
using Xunit;

namespace NKristek.Smaragd.Tests.Validation
{
    public class PredicateValidationTests
    {
        private string ErrorMessage { get; }

        private PredicateValidation<int> Validation { get; }

        public PredicateValidationTests()
        {
            ErrorMessage = "Value should be at least 5.";
            Validation = new PredicateValidation<int>(i => i >= 5, ErrorMessage);
        }

        [Fact]
        public void PredicateValidation_PredicateNullThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PredicateValidation<int>(null, "error"));
        }

        [Fact]
        public void PredicateValidation_ErrorMessageNullThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PredicateValidation<int>(i => i >= 5, null));
        }

        [Theory]
        [InlineData(4, false)]
        [InlineData(5, true)]
        public void IsValid_ReturnsExpectedResult(int input, bool expectedResult)
        {
            var result = Validation.IsValid(input, out _);
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(4, "Value should be at least 5.")]
        [InlineData(5, null)]
        public void IsValid_ReturnsExpectedErrorMessage(int input, string expectedErrorMessage)
        {
            Validation.IsValid(input, out var errorMessage);
            Assert.Equal(expectedErrorMessage, errorMessage);
        }
    }
}
