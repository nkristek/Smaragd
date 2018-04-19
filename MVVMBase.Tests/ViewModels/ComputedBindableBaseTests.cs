using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nkristek.MVVMBase.Commands;
using nkristek.MVVMBase.ViewModels;

namespace nkristek.MVVMBase.Tests.ViewModels
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
                set => SetProperty(ref _testProperty, value);
            }
            
            [PropertySource(nameof(TestProperty))]
            public bool AnotherTestProperty => _testProperty;

            [CommandCanExecuteSource(nameof(TestProperty))]
            public IRaiseCanExecuteChanged TestCommand { get; set; }

            public int OnPropertyExecutionCount;

            protected override void OnPropertyChanged(string propertyName = null)
            {
                base.OnPropertyChanged(propertyName);
                OnPropertyExecutionCount++;
            }
        }
        
        [TestMethod]
        public void TestPropertySourceAttribute()
        {
            var invokedPropertyChangedEvents = new List<string>();

            var bindableObject = new ComputedBindableBaseTest();
            bindableObject.PropertyChanged += (sender, e) =>
            {
                invokedPropertyChangedEvents.Add(e.PropertyName);
            };

            bindableObject.TestProperty = true;

            Assert.AreEqual(2, invokedPropertyChangedEvents.Count, "Invalid count of invocations of the PropertyChanged event");
            Assert.IsTrue(invokedPropertyChangedEvents.Contains(nameof(ComputedBindableBaseTest.AnotherTestProperty)), "The PropertyChanged event wasn't raised for the PropertySource property");
            Assert.AreEqual(2, bindableObject.OnPropertyExecutionCount, "Invalid count of invocations of OnPropertyChanged");
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
