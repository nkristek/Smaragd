using Microsoft.VisualStudio.TestTools.UnitTesting;
using NKristek.Smaragd.Validation;

namespace NKristek.Smaragd.Tests.Validation
{
    [TestClass]
    public class PredicateValidationTests
    {
        [TestMethod]
        public void TestIsValid()
        {
            var errorMessage = "Value should be at least 5.";
            var validation = new PredicateValidation<int>(i => i >= 5, errorMessage);

            Assert.IsFalse(validation.IsValid(4, out var returnedErrorMessage), "IsValid did not return false.");
            Assert.AreEqual(errorMessage, returnedErrorMessage, "The returned error message does not match.");

            Assert.IsTrue(validation.IsValid(5, out returnedErrorMessage), "IsValid did not return true.");
            Assert.IsNull(returnedErrorMessage, "The returned error message is not null.");
        }
    }
}
