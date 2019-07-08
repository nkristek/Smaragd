using NKristek.Smaragd.Attributes;
using Xunit;

namespace NKristek.Smaragd.Tests.Attributes
{
    public class IsReadOnlyIgnoredAttributeTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void InheritAttributes(bool value)
        {
            var attribute = new IsReadOnlyIgnoredAttribute
            {
                InheritAttributes = value
            };
            Assert.Equal(value, attribute.InheritAttributes);
        }
    }
}
