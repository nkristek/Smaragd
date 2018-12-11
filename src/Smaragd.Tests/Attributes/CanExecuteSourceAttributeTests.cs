using System;
using NKristek.Smaragd.Attributes;
using Xunit;

namespace NKristek.Smaragd.Tests.Attributes
{
    public class CanExecuteSourceAttributeTests
    {
        private string[] PropertySources { get; }

        private CanExecuteSourceAttribute Attribute { get; }

        public CanExecuteSourceAttributeTests()
        {
            PropertySources = new[] {"FirstProperty", "SecondProperty"};
            Attribute = new CanExecuteSourceAttribute(PropertySources);
        }

        [Fact]
        public void CanExecuteSourceAttribute()
        {
            Assert.Equal(PropertySources, Attribute.PropertySources);
        }

        [Fact]
        public void CanExecuteSourceAttribute_PropertySourcesNullThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CanExecuteSourceAttribute(null));
        }
    }
}