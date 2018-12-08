using System;
using NKristek.Smaragd.Attributes;
using Xunit;

namespace NKristek.Smaragd.Tests.Attributes
{
    public class PropertySourceAttributeTests
    {
        private string[] PropertySources { get; }

        private PropertySourceAttribute Attribute { get; }

        public PropertySourceAttributeTests()
        {
            PropertySources = new[] { "FirstProperty", "SecondProperty" };
            Attribute = new PropertySourceAttribute(PropertySources);
        }

        [Fact]
        public void PropertySourceAttribute()
        {
            Assert.Equal(PropertySources, Attribute.PropertySources);
        }

        [Fact]
        public void PropertySourceAttribute_PropertySourcesNullThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PropertySourceAttribute(null));
        }
    }
}
