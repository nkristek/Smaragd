using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NKristek.Smaragd.Commands;

namespace NKristek.Smaragd.Tests.Commands
{
    /// <summary>
    /// Summary description for BindableCommand
    /// </summary>
    [TestClass]
    public class BindableCommandTests
    {
        private class TestCommand
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
        }
    }
}
