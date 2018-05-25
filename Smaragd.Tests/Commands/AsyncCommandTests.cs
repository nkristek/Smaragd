using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NKristek.Smaragd.Commands;

namespace NKristek.Smaragd.Tests.Commands
{
    /// <summary>
    /// Summary description for AsyncCommandTests
    /// </summary>
    [TestClass]
    public class AsyncCommandTests
    {
        private class TestIsWorkingCommand
            : AsyncCommand
        {
            public override bool CanExecute(object parameter)
            {
                return parameter != null;
            }

            protected override async Task DoExecute(object parameter)
            {
                await Task.Run(() => Thread.Sleep(500));
            }
        }

        private class TestOnThrownExceptionCommand
            : AsyncCommand
        {
            public override bool CanExecute(object parameter)
            {
                return parameter != null;
            }

            protected override Task DoExecute(object parameter)
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
        public void TestIsWorking()
        {
            var command = new TestIsWorkingCommand();
            Assert.IsFalse(command.IsWorking, "Command.IsWorking is not false before executing");
            var executeTask = command.ExecuteAsync(null);
            Assert.IsTrue(command.IsWorking, "Command.IsWorking is not set to true while executing");
            executeTask.Wait();
            Assert.IsFalse(command.IsWorking, "Command.IsWorking is not false after executing");
        }

        [TestMethod]
        public void TestOnThrownException()
        {
            var command = new TestOnThrownExceptionCommand();
            command.ExecuteAsync(null).Wait();
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
