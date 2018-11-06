using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Tests.ViewModels
{
    [TestClass]
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

        [TestMethod]
        public void TestSetProperty()
        {
            var bindableObject = new BindableBaseTest
            {
                TestProperty = true
            };
            Assert.IsFalse(bindableObject.OldValue, "Old value has to be false.");
            Assert.IsTrue(bindableObject.ValueWasSet, "The value was not set.");
            Assert.IsTrue(bindableObject.TestProperty, "The value of the property is wrong.");
            Assert.ThrowsException<ArgumentNullException>(() => bindableObject.SetPropertyExternal(ref bindableObject._testProperty, false, out _, null), "An empty property name should raise an exception.");
        }

        [TestMethod]
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
            
            Assert.AreEqual(1, invokedPropertyChangedEvents.Count, "Invalid count of invocations on INotifyPropertyChanged.PropertyChanged.");
            Assert.AreEqual("TestProperty", invokedPropertyChangedEvents.FirstOrDefault(), "No event on INotifyPropertyChanged.PropertyChanged was raised for the test property");
        }
    }
}
