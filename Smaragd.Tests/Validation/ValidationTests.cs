using System;
using Xunit;
using NKristek.Smaragd.Validation;

namespace NKristek.Smaragd.Tests.Validation
{
    public class ValidationTests
    {
        private class TestValidation
            : Validation<int>
        {
            private readonly string _errorMessage;
            
            public TestValidation(string errorMessage)
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

        private class TestStringValidation
            : Validation<string>
        {
            private readonly string _errorMessage;

            public TestStringValidation(string errorMessage)
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

        [Fact]
        public void TestIsValid()
        {
            var errorMessage = "Value should be at least 5.";
            var validation = new TestValidation(errorMessage);

            Assert.False(validation.IsValid(4, out var returnedErrorMessage), "IsValid did not return false.");
            Assert.Equal(errorMessage, returnedErrorMessage);

            Assert.True(validation.IsValid(5, out returnedErrorMessage), "IsValid did not return true.");
            Assert.Null(returnedErrorMessage);

            // an invalid argument should raise an exception
            Assert.Throws<ArgumentException>(() => validation.IsValid("error", out returnedErrorMessage));
            Assert.Throws<ArgumentException>(() => validation.IsValid(null, out returnedErrorMessage));
        }

        [Fact]
        public void TestIsValidString()
        {
            var errorMessage = "Value should not be null or empty.";
            var validation = new TestStringValidation(errorMessage);

            Assert.Throws<ArgumentException>(() => validation.IsValid(4, out _));
            Assert.False(validation.IsValid(null, out _), "IsValid did not return false.");
            Assert.True(validation.IsValid("Test", out _), "IsValid did not return true.");
            Assert.Throws<ArgumentException>(() => validation.IsValid(new object(), out _));
        }
    }
}
