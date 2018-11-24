using System;
using Xunit;
using NKristek.Smaragd.Commands;

namespace NKristek.Smaragd.Tests.Commands
{
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

        [Fact]
        public void TestCanExecuteChanged()
        {
            var command = new TestCommand();
            var canExecuteChangedInvokedCount = 0;
            command.CanExecuteChanged += (sender, e) => { canExecuteChangedInvokedCount++; };
            command.RaiseCanExecuteChanged();
            // CanExecuteChanged should have been raised 1 time
            Assert.Equal(1, canExecuteChangedInvokedCount);
        }

        [Fact]
        public void TestCanExecute()
        {
            var command = new TestCommand();
            Assert.True(command.CanExecute(new object()), "CanExecute did not return true");
            Assert.False(command.CanExecute(null), "CanExecute did not return false");

            // test the default implementation of CanExecute()
            var defaultCanExecuteCommand = new TestDefaultCanExecuteCommand();
            Assert.True(defaultCanExecuteCommand.CanExecute(null), "CanExecute did not return true by default");
        }

        [Fact]
        public void TestExecute()
        {
            var command = new TestCommand();
            Assert.False(command.DidExecute, "DidExecute is already true");

            command.Execute(null);
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
