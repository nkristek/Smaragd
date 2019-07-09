using NKristek.Smaragd.Attributes;
using Xunit;

namespace NKristek.Smaragd.Tests.Attributes
{
    public class IsDirtyIgnoredAttributeTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void InheritAttributes(bool value)
        {
            var attribute = new IsDirtyIgnoredAttribute
            {
                InheritAttributes = value
            };
            Assert.Equal(value, attribute.InheritAttributes);
        }
    }
}
