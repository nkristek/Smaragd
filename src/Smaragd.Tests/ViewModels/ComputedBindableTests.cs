using System;
using System.Collections.Generic;
using System.Linq;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.ViewModels;
using Xunit;

namespace NKristek.Smaragd.Tests.ViewModels
{
    public class ComputedBindableTests
    {
        private class PropertySourceComputedBindable
            : ComputedBindable
        {
            private bool _testProperty;

            public bool TestProperty
            {
                get => _testProperty;
                set => SetProperty(ref _testProperty, value, out _);
            }

            [PropertySource(nameof(TestProperty))] public bool AnotherTestProperty => TestProperty;

            [PropertySource("NotExistingProperty")]
            public bool TestPropertyWithNonExistentSource { get; }

            [PropertySource(nameof(TestPropertyWithSelfReference))]
            public bool TestPropertyWithSelfReference { get; }

            public void RaisePropertyChangedExternal(string propertyName)
            {
                RaisePropertyChanged(propertyName);
            }

            public void RaisePropertyChangedExternal(string propertyName, IEnumerable<string> additionalPropertyNames)
            {
                RaisePropertyChanged(propertyName, additionalPropertyNames);
            }
        }

        [Fact]
        public void PropertySourceAttribute_raises_event_on_PropertyChanged()
        {
            var invokedPropertyChangedEvents = new List<string>();
            var computedBindable = new PropertySourceComputedBindable();
            computedBindable.PropertyChanged += (sender, e) => invokedPropertyChangedEvents.Add(e.PropertyName);
            computedBindable.TestProperty = true;
            var expectedPropertyChangedEvents = new List<string>
            {
                nameof(PropertySourceComputedBindable.TestProperty),
                nameof(PropertySourceComputedBindable.AnotherTestProperty)
            };
            Assert.Equal(expectedPropertyChangedEvents, invokedPropertyChangedEvents);
        }

        [Theory]
        [InlineData(nameof(PropertySourceComputedBindable.AnotherTestProperty), true)]
        [InlineData(nameof(PropertySourceComputedBindable.TestProperty), false)]
        [InlineData("NotExistingProperty", false)]
        public void PropertyNameHasAttribute(string propertyName, bool expectedResult)
        {
            var computedBindable = new PropertySourceComputedBindable();
            Assert.Equal(expectedResult, computedBindable.PropertyNameHasAttribute<PropertySourceAttribute>(propertyName));
        }

        [Fact]
        public void RaisePropertyChanged_propertyName_is_null_throws_ArgumentNullException()
        {
            var computedBindable = new PropertySourceComputedBindable();
            Assert.Throws<ArgumentNullException>(() => computedBindable.RaisePropertyChangedExternal(null));
            Assert.Throws<ArgumentNullException>(() => computedBindable.RaisePropertyChangedExternal(null, Enumerable.Empty<string>()));
        }
    }
}