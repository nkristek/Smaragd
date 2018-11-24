using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xunit;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Tests.ViewModels
{
    public class BindableBaseTests
    {
        private class BindableBaseTest 
            : BindableBase
        {
            public bool _testProperty;

            public bool TestProperty
            {
                get => _testProperty;
                set
                {
                    ValueWasSet = SetProperty(ref _testProperty, value, out var oldValue);
                    OldValue = oldValue;
                }
            }

            public bool ValueWasSet;

            public bool OldValue;

            public bool SetPropertyExternal<T>(ref T storage, T value, out T oldValue, string propertyName = "")
            {
                return SetProperty(ref storage, value, out oldValue, propertyName);
            }
        }

        [Fact]
        public void TestSetProperty()
        {
            var bindableObject = new BindableBaseTest
            {
                TestProperty = true
            };
            Assert.False(bindableObject.OldValue, "Old value has to be false.");
            Assert.True(bindableObject.ValueWasSet, "The value was not set.");
            Assert.True(bindableObject.TestProperty, "The value of the property is wrong.");

            // An empty property name should raise an exception.
            Assert.Throws<ArgumentNullException>(() => bindableObject.SetPropertyExternal(ref bindableObject._testProperty, false, out _, null));
        }

        [Fact]
        public void TestPropertyChanged()
        {
            var invokedPropertyChangedEvents = new List<string>();

            var bindableObject = new BindableBaseTest();
            bindableObject.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                invokedPropertyChangedEvents.Add(e.PropertyName);
            };

            bindableObject.TestProperty = true;
            bindableObject.TestProperty = true;
            
            Assert.Single(invokedPropertyChangedEvents);
            Assert.Equal("TestProperty", invokedPropertyChangedEvents.FirstOrDefault());
        }
    }
}
