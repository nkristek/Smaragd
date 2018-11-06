using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NKristek.Smaragd.Commands;

namespace NKristek.Smaragd.Tests.Commands
{
    [TestClass]
    public class BindableCommandTests
    {
        private class TestCommand
            : BindableCommand
        {
            public bool DidExecute { get; private set; }

            public override bool CanExecute(object parameter)
            {
                return parameter != null;
            }

            protected override void DoExecute(object parameter)
            {
                DidExecute = true;
            }
        }

        private class TestDefaultCanExecuteCommand
            : BindableCommand
        {
            protected override void DoExecute(object parameter)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void TestCanExecuteChanged()
        {
            var command = new TestCommand();
            var canExecuteChangedInvokedCount = 0;
            command.CanExecuteChanged += (sender, e) => { canExecuteChangedInvokedCount++; };
            command.RaiseCanExecuteChanged();
            Assert.AreEqual(1, canExecuteChangedInvokedCount, "Invalid count of invocations of the CanExecuteChanged event");
        }

        [TestMethod]
        public void TestCanExecute()
        {
            var command = new TestCommand();
            Assert.IsTrue(command.CanExecute(new object()), "CanExecute did not return true");
            Assert.IsFalse(command.CanExecute(null), "CanExecute did not return false");

            // test the default implementation of CanExecute()
            var defaultCanExecuteCommand = new TestDefaultCanExecuteCommand();
            Assert.IsTrue(defaultCanExecuteCommand.CanExecute(null), "CanExecute did not return true by default");
        }

        [TestMethod]
        public void TestExecute()
        {
            var command = new TestCommand();
            Assert.IsFalse(command.DidExecute, "DidExecute is already true");

            command.Execute(null);
            Assert.IsTrue(command.DidExecute, "DidExecute was not set to true");
        }

        [TestMethod]
        public void TestRaiseCanExecuteChanged()
        {
            var command = new TestCommand();
            var canExecuteChangedInvokedCount = 0;
            command.CanExecuteChanged += (sender, e) => { canExecuteChangedInvokedCount++; };
            command.RaiseCanExecuteChanged();
            Assert.AreEqual(1, canExecuteChangedInvokedCount, "Invalid count of invocations of the CanExecuteChanged event");
        }
    }
}
