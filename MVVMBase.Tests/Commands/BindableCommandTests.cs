using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nkristek.MVVMBase.Commands;

namespace nkristek.MVVMBase.Tests.Commands
{
    /// <summary>
    /// Summary description for BindableCommand
    /// </summary>
    [TestClass]
    public class BindableCommandTests
    {
        private class TestOnThrownExceptionCommand
            : BindableCommand
        {
            public override bool CanExecute(object parameter)
            {
                return parameter != null;
            }

            protected override void DoExecute(object parameter)
            {
                throw new Exception();
            }

            public bool OnThrownExceptionWasCalled { get; private set; }

            protected override void OnThrownException(object parameter, Exception exception)
            {
                OnThrownExceptionWasCalled = true;
            }
        }

        [TestMethod]
        public void TestOnThrownException()
        {
            var command = new TestOnThrownExceptionCommand();
            command.Execute(null);
            Assert.IsTrue(command.OnThrownExceptionWasCalled, "OnThrownException was not called");
        }

        [TestMethod]
        public void TestCanExecuteChanged()
        {
            var command = new TestOnThrownExceptionCommand();
            var canExecuteChangedInvokedCount = 0;
            command.CanExecuteChanged += (sender, e) => { canExecuteChangedInvokedCount++; };
            command.RaiseCanExecuteChanged();
            Assert.AreEqual(1, canExecuteChangedInvokedCount, "Invalid count of invocations of the CanExecuteChanged event");
        }

        [TestMethod]
        public void TestCanExecute()
        {
            var command = new TestOnThrownExceptionCommand();
            Assert.IsTrue(command.CanExecute(new object()), "CanExecute did not return true");
            Assert.IsFalse(command.CanExecute(null), "CanExecute did not return false");
        }
    }
}
