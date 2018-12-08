using System;
using NKristek.Smaragd.Validation;
using Xunit;

namespace NKristek.Smaragd.Tests.Validation
{
    public class ValidationTests
    {
        private class IntAtLeast5Validation
            : Validation<int>
        {
            private readonly string _errorMessage;
            
            public IntAtLeast5Validation(string errorMessage)
            {
                _errorMessage = errorMessage;
            }

            public override bool IsValid(int value, out string errorMessage)
            {
                errorMessage = null;
                if (value >= 5)
                    return true;

                errorMessage = _errorMessage;
                return false;
            }
        }

        private class StringNotNullOrEmptyValidation
            : Validation<string>
        {
            private readonly string _errorMessage;

            public StringNotNullOrEmptyValidation(string errorMessage)
            {
                _errorMessage = errorMessage;
            }

            public override bool IsValid(string value, out string errorMessage)
            {
                errorMessage = null;
                if (!String.IsNullOrEmpty(value))
                    return true;

                errorMessage = _errorMessage;
                return false;
            }
        }

        private string IntValidationErrorMessage { get; }

        private Validation<int> IntValidation { get; }

        private string StringValidationErrorMessage { get; }

        private Validation<string> StringValidation { get; }

        public ValidationTests()
        {
            IntValidationErrorMessage = "Value should be at least 5.";
            IntValidation = new IntAtLeast5Validation(IntValidationErrorMessage);

            StringValidationErrorMessage = "Value should not be null or empty.";
            StringValidation = new StringNotNullOrEmptyValidation(StringValidationErrorMessage);
        }
        
        [Theory]
        [InlineData(4, false)]
        [InlineData(5, true)]
        public void StructValidation_IsValid_ReturnsExpectedResult(int input, bool expectedResult)
        {
            var result = ((IValidation)IntValidation).IsValid(input, out _);
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(4, "Value should be at least 5.")]
        [InlineData(5, null)]
        public void StructValidation_IsValid_ReturnsExpectedErrorMessage(int input, string expectedErrorMessage)
        {
            ((IValidation)IntValidation).IsValid(input, out var errorMessage);
            Assert.Equal(expectedErrorMessage, errorMessage);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("error")]
        public void StructValidation_IsValid_InvalidArgumentThrowsArgumentException(object input)
        {
            Assert.Throws<ArgumentException>(() => ((IValidation)IntValidation).IsValid(input, out _));
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("Not empty", true)]
        public void ClassValidation_IsValid_ReturnsExpectedResult(string input, bool expectedResult)
        {
            var result = ((IValidation)StringValidation).IsValid(input, out _);
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(null, "Value should not be null or empty.")]
        [InlineData("Not empty", null)]
        public void ClassValidation_IsValid_ReturnsExpectedErrorMessage(string input, string expectedErrorMessage)
        {
            ((IValidation)StringValidation).IsValid(input, out var errorMessage);
            Assert.Equal(expectedErrorMessage, errorMessage);
        }

        [Fact]
        public void ClassValidation_IsValid_InvalidArgumentThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => ((IValidation)StringValidation).IsValid(new object(), out _));
        }
    }
}
