using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Tests.ViewModels
{
    [TestClass]
    public class ComputedBindableBaseTests
    {
        private class PropertySourceTest
            : ComputedBindableBase
        {
            private bool _testProperty;

            public bool TestProperty
            {
                get => _testProperty;
                set => SetProperty(ref _testProperty, value, out _);
            }

            [PropertySource(nameof(TestProperty))]
            public bool AnotherTestProperty => TestProperty;
        }

        private class CanExecuteSourceTest
            : ComputedBindableBase
        {
            private bool _testProperty;

            public bool TestProperty
            {
                get => _testProperty;
                set => SetProperty(ref _testProperty, value, out _);
            }

            public BindableCommand TestCommand { get; } = new CanExecuteSourceTestCommand();

            public AsyncBindableCommand AsyncTestCommand { get; } = new AsyncCanExecuteSourceTestCommand();
        }
        
        private class CanExecuteSourceTestCommand
            : BindableCommand
        {
            // define the CanExecuteSourceAttribute on a protected method instead to test if it also works on protected methods.
            //[CanExecuteSource(nameof(CanExecuteSourceTest.TestProperty))]
            public override bool CanExecute(object parameter)
            {
                return parameter is CanExecuteSourceTest test && test.TestProperty;
            }

            [CanExecuteSource(nameof(CanExecuteSourceTest.TestProperty))]
            protected bool CanExecute()
            {
                return true;
            }

            protected override void DoExecute(object parameter)
            {
                throw new System.NotImplementedException();
            }
        }

        private class AsyncCanExecuteSourceTestCommand
            : AsyncBindableCommand
        {
            [CanExecuteSource(nameof(CanExecuteSourceTest.TestProperty))]
            public override bool CanExecute(object parameter)
            {
                return parameter is CanExecuteSourceTest test && test.TestProperty;
            }

            protected override async Task DoExecute(object parameter)
            {
                await Task.Run(() => throw new System.NotImplementedException());
            }
        }

        [TestMethod]
        public void TestPropertySourceAttribute()
        {
            var invokedPropertyChangedEvents = new List<string>();

            var bindableObject = new PropertySourceTest();
            bindableObject.PropertyChanged += (sender, e) => { invokedPropertyChangedEvents.Add(e.PropertyName); };
            bindableObject.TestProperty = true;

            Assert.AreEqual(2, invokedPropertyChangedEvents.Count, "Invalid count of invocations of the PropertyChanged event");
            Assert.IsTrue(invokedPropertyChangedEvents.Contains(nameof(PropertySourceTest.TestProperty)), "The PropertyChanged event wasn't raised for the TestProperty property");
            Assert.IsTrue(invokedPropertyChangedEvents.Contains(nameof(PropertySourceTest.AnotherTestProperty)), "The PropertyChanged event wasn't raised for the PropertySource property");
        }
        
        [TestMethod]
        public void TestCommandCanExecuteSourceAttribute()
        {
            var testObject = new CanExecuteSourceTest();
            Assert.IsFalse(testObject.TestCommand.CanExecute(testObject), "CanExecute() should return false when TestProperty is false");

            var invokedCanExecuteChangedEvents = 0;
            testObject.TestCommand.CanExecuteChanged += (sender, e) => { invokedCanExecuteChangedEvents++; };

            var asyncInvokedCanExecuteChangedEvents = 0;
            testObject.AsyncTestCommand.CanExecuteChanged += (sender, e) => { asyncInvokedCanExecuteChangedEvents++; };

            testObject.TestProperty = true;
            Assert.IsTrue(testObject.TestCommand.CanExecute(testObject), "CanExecute() should return true when TestProperty is true");
            Assert.AreEqual(1, invokedCanExecuteChangedEvents, "Invalid count of invocations of CanExecuteChanged of the BindableCommand");
            Assert.AreEqual(1, asyncInvokedCanExecuteChangedEvents, "Invalid count of invocations of CanExecuteChanged of the AsyncBindableCommand");
        }
    }
}
