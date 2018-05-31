using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Tests.ViewModels
{
    /// <summary>
    /// Summary description for ComputedBindableBaseTests
    /// </summary>
    [TestClass]
    public class ComputedBindableBaseTests
    {
        private class ComputedBindableBaseTest
            : ComputedBindableBase
        {
            private bool _testProperty;
            public bool TestProperty
            {
                get => _testProperty;
                set => SetProperty(ref _testProperty, value, out _);
            }
            
            [PropertySource(nameof(TestProperty))]
            public bool AnotherTestProperty => _testProperty;

            [CommandCanExecuteSource(nameof(TestProperty))]
            public IRaiseCanExecuteChanged TestCommand { get; set; }

            public ObservableCollection<int> MyValues { get; } = new ObservableCollection<int>();

            [PropertySource(nameof(MyValues), NotifyCollectionChangedAction.Add, NotifyCollectionChangedAction.Remove, NotifyCollectionChangedAction.Replace, NotifyCollectionChangedAction.Reset)]
            public int MinValue => MyValues.Min();

            [PropertySource(nameof(MyValues), NotifyCollectionChangedAction.Add, NotifyCollectionChangedAction.Remove, NotifyCollectionChangedAction.Replace, NotifyCollectionChangedAction.Reset)]
            public int MaxValue => MyValues.Max();
        }
        
        [TestMethod]
        public void TestPropertySourceAttribute()
        {
            var invokedPropertyChangedEvents = new List<string>();

            var bindableObject = new ComputedBindableBaseTest();
            bindableObject.MyValues.Add(1);
            bindableObject.PropertyChanged += (sender, e) =>
            {
                invokedPropertyChangedEvents.Add(e.PropertyName);
            };

            bindableObject.TestProperty = true;

            Assert.AreEqual(2, invokedPropertyChangedEvents.Count, "Invalid count of invocations of the PropertyChanged event");
            Assert.IsTrue(invokedPropertyChangedEvents.Contains(nameof(ComputedBindableBaseTest.AnotherTestProperty)), "The PropertyChanged event wasn't raised for the PropertySource property");
            
            bindableObject.MyValues.Add(2);
            Assert.AreEqual(4, invokedPropertyChangedEvents.Count, "Invalid count of invocations of the PropertyChanged event");
            Assert.IsTrue(invokedPropertyChangedEvents.Contains(nameof(ComputedBindableBaseTest.MaxValue)), "The PropertyChanged event wasn't raised for the PropertySource property");
        }

        [TestMethod]
        public void TestCommandCanExecuteSourceAttribute()
        {
            var invokedCanExecuteChangedEvents = 0;
            
            var testCommand = new RelayCommand(o => { });
            testCommand.CanExecuteChanged += (sender, e) => { invokedCanExecuteChangedEvents++; };

            var bindableObject = new ComputedBindableBaseTest
            {
                TestCommand = testCommand,
                TestProperty = true
            };
            
            Assert.AreEqual(1, invokedCanExecuteChangedEvents, "Invalid count of invocations of CanExecuteChanged");
        }
    }
}
