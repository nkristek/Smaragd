using System;
using NKristek.Smaragd.Attributes;
using Xunit;

namespace NKristek.Smaragd.Tests.Attributes
{
    public class PropertySourceAttributeTests
    {
        [Fact]
        public void Constructor_propertyNames_null_throws_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PropertySourceAttribute(null));
        }

        [Fact]
        public void Constructor_sets_PropertySources()
        {
            var propertyNames = new[] { "FirstProperty", "SecondProperty" };
            var attribute = new PropertySourceAttribute(propertyNames);
            Assert.Equal(propertyNames, attribute.PropertySources);
        }

        [Fact]
        public void PropertySources()
        {
            var propertyNames = new[] { "FirstProperty", "SecondProperty" };
            var attribute = new PropertySourceAttribute
            {
                PropertySources = propertyNames
            };
            Assert.Equal(propertyNames, attribute.PropertySources);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void InheritAttributes(bool value)
        {
            var attribute = new PropertySourceAttribute
            {
                InheritAttributes = value
            };
            Assert.Equal(value, attribute.InheritAttributes);
        }
    }
}