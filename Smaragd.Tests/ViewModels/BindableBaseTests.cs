using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Tests.ViewModels
{
    /// <summary>
    /// Summary description for BindableBaseTests
    /// </summary>
    [TestClass]
    public class BindableBaseTests
    {
        private class BindableBaseTest 
            : BindableBase
        {
            private bool _testProperty;
            public bool TestProperty
            {
                get => _testProperty;
                set => SetProperty(ref _testProperty, value, out _);
            }
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

            Assert.IsTrue(bindableObject.TestProperty, "Property wasn't set");
            Assert.AreEqual(1, invokedPropertyChangedEvents.Count, "Invalid count of invocations of the PropertyChanged event");
            Assert.AreEqual("TestProperty", invokedPropertyChangedEvents.FirstOrDefault(), "The PropertyChanged event wasn't raised for the test property");
        }
    }
}
