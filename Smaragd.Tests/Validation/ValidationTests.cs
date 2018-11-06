using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NKristek.Smaragd.Validation;

namespace NKristek.Smaragd.Tests.Validation
{
    [TestClass]
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

        [TestMethod]
        public void TestIsValid()
        {
            var errorMessage = "Value should be at least 5.";
            var validation = new TestValidation(errorMessage);

            Assert.IsFalse(validation.IsValid(4, out var returnedErrorMessage), "IsValid did not return false.");
            Assert.AreEqual(errorMessage, returnedErrorMessage, "The returned error message does not match.");

            Assert.IsTrue(validation.IsValid(5, out returnedErrorMessage), "IsValid did not return true.");
            Assert.IsNull(returnedErrorMessage, "The returned error message is not null.");

            Assert.ThrowsException<ArgumentException>(() => validation.IsValid("error", out returnedErrorMessage), "An invalid argument type did not raise an exception.");
            Assert.ThrowsException<ArgumentException>(() => validation.IsValid(null, out returnedErrorMessage), "An invalid argument type did not raise an exception.");
        }

        [TestMethod]
        public void TestIsValidString()
        {
            var errorMessage = "Value should not be null or empty.";
            var validation = new TestStringValidation(errorMessage);

            Assert.ThrowsException<ArgumentException>(() => validation.IsValid(4, out _), "IsValid did not return false.");
            Assert.IsFalse(validation.IsValid(null, out _), "IsValid did not return false.");
            Assert.IsTrue(validation.IsValid("Test", out _), "IsValid did not return true.");
            Assert.ThrowsException<ArgumentException>(() => validation.IsValid(new object(), out _), "An invalid argument type did not raise an exception.");
        }
    }
}
