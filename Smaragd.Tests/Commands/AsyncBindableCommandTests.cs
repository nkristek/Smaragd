using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NKristek.Smaragd.Commands;

namespace NKristek.Smaragd.Tests.Commands
{
    [TestClass]
    public class AsyncBindableCommandTests
    {
        private class TestIsWorkingCommand
            : AsyncBindableCommand
        {
            protected override async Task DoExecute(object parameter)
            {
                await Task.Run(() => Thread.Sleep(500));
            }
        }

        private class TestCommand
            : AsyncBindableCommand
        {
            public bool DidExecute { get; private set; }

            public override bool CanExecute(object parameter)
            {
                return parameter != null;
            }

            protected override async Task DoExecute(object parameter)
            {
                await Task.Run(() => DidExecute = true);
            }
        }

        [TestMethod]
        public async Task TestIsWorking()
        {
            var command = new TestIsWorkingCommand();
            Assert.IsFalse(command.IsWorking, "Command.IsWorking is not false before executing");
            
            var executeTask = command.ExecuteAsync(null);
            Assert.IsTrue(command.IsWorking, "Command.IsWorking is not set to true while executing");
            
            await executeTask;
            Assert.IsFalse(command.IsWorking, "Command.IsWorking is not false after executing");
        }

        [TestMethod]
        public async Task TestCanExecute()
        {
            var command = new TestCommand();
            Assert.IsTrue(command.CanExecute(new object()), "CanExecute did not return true");
            Assert.IsFalse(command.CanExecute(null), "CanExecute did not return false");

            // test the default implementation of CanExecute()
            var isWorkingCommand = new TestIsWorkingCommand();
            Assert.IsTrue(isWorkingCommand.CanExecute(null), "CanExecute did not return true although the command is not working");

            var executeTask = isWorkingCommand.ExecuteAsync(null);
            Assert.IsFalse(isWorkingCommand.CanExecute(null), "CanExecute did not return false although the command is working");

            await executeTask;
        }

        [TestMethod]
        public async Task TestExecuteAsync()
        {
            var command = new TestCommand();
            Assert.IsFalse(command.DidExecute, "DidExecute is already true");

            await command.ExecuteAsync(null);
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
