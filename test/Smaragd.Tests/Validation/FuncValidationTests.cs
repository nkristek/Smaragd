using System;
using NKristek.Smaragd.Validation;
using Xunit;

namespace NKristek.Smaragd.Tests.Validation
{
    public class FuncValidationTests
    {
        [Fact]
        public void FuncValidation_Func_null_throws_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new FuncValidation<int, bool>(null));
        }

        [Theory]
        [InlineData(4, false)]
        [InlineData(5, true)]
        public void IsValid_ReturnsExpectedResult(int input, bool expectedResult)
        {
            var validation = new FuncValidation<int, bool>(i => i >= 5);
            var result = validation.Validate(input);
            Assert.Equal(expectedResult, result);
        }
    }
}