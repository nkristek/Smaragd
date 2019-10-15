using System;
using NKristek.Smaragd.Validation;
using Xunit;

namespace NKristek.Smaragd.Tests.Validation
{
    public class PredicateValidationTests
    {
        [Fact]
        public void PredicateValidation_Func_null_throws_ArgumentNullException()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(() => new PredicateValidation<int>(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Theory]
        [InlineData(4, false)]
        [InlineData(5, true)]
        public void IsValid_ReturnsExpectedResult(int input, bool expectedResult)
        {
            var validation = new PredicateValidation<int>(i => i >= 5);
            var result = validation.Validate(input);
            Assert.Equal(expectedResult, result);
        }
    }
}
