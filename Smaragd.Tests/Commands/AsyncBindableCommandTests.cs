using System.Threading;
using System.Threading.Tasks;
using NKristek.Smaragd.Commands;
using Xunit;

namespace NKristek.Smaragd.Tests.Commands
{
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

        [Fact]
        public async Task TestIsWorking()
        {
            var command = new TestIsWorkingCommand();
            Assert.False(command.IsWorking, "Command.IsWorking is not false before executing");

            var executeTask = command.ExecuteAsync(null);
            Assert.True(command.IsWorking, "Command.IsWorking is not set to true while executing");

            await executeTask;
            Assert.False(command.IsWorking, "Command.IsWorking is not false after executing");
        }

        [Fact]
        public async Task TestCanExecute()
        {
            var command = new TestCommand();
            Assert.True(command.CanExecute(new object()), "CanExecute did not return true");
            Assert.False(command.CanExecute(null), "CanExecute did not return false");

            // test the default implementation of CanExecute()
            var isWorkingCommand = new TestIsWorkingCommand();
            Assert.True(isWorkingCommand.CanExecute(null), "CanExecute did not return true although the command is not working");

            var executeTask = isWorkingCommand.ExecuteAsync(null);
            Assert.False(isWorkingCommand.CanExecute(null), "CanExecute did not return false although the command is working");

            await executeTask;
        }

        [Fact]
        public async Task TestExecuteAsync()
        {
            var command = new TestCommand();
            Assert.False(command.DidExecute, "DidExecute is already true");

            await command.ExecuteAsync(null);
            Assert.True(command.DidExecute, "DidExecute was not set to true");
        }

        [Fact]
        public void TestRaiseCanExecuteChanged()
        {
            var command = new TestCommand();
            var canExecuteChangedInvokedCount = 0;
            command.CanExecuteChanged += (sender, e) => { canExecuteChangedInvokedCount++; };
            command.RaiseCanExecuteChanged();
            // CanExecuteChanged should have been raised 1 time
            Assert.Equal(1, canExecuteChangedInvokedCount);
        }
    }
}
