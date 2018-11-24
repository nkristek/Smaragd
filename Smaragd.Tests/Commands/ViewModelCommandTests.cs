using System;
using Xunit;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Tests.Commands
{
    public class ViewModelCommandTests
    {
        private class TestViewModel
            : ViewModel
        {

        }

        private class TestCommand
            : ViewModelCommand<TestViewModel>
        {
            public TestCommand(TestViewModel parent) : base(parent) { }

            protected override void DoExecute(TestViewModel viewModel, object parameter)
            {
                if (viewModel == null)
                    throw new ArgumentNullException(nameof(viewModel));

                if (parameter == null)
                    throw new ArgumentNullException(nameof(parameter));

                if (viewModel != parameter)
                    throw new Exception("invalid parameter");
            }
        }
        
        [Fact]
        public void TestParent()
        {
            var viewModel = new TestViewModel();
            var command = new TestCommand(viewModel);
            Assert.Equal(viewModel, command.Parent);
        }

        [Fact]
        public void TestNoParentException()
        {
            // AsyncViewModelCommand should throw an exception when instancing with no parent
            Assert.Throws<ArgumentNullException>(() => new TestCommand(null));
        }

        [Fact]
        public void TestCanExecute()
        {
            Assert.True(new TestCommand(new TestViewModel()).CanExecute(null), "CanExecute() should return true by default.");
        }

        [Fact]
        public void TestDoExecute()
        {
            var viewModel = new TestViewModel();
            var command = new TestCommand(viewModel);
            command.Execute(viewModel);
            // Execute() should throw an exception when the parameter is null.
            Assert.Throws<ArgumentNullException>(() => command.Execute(null));
        }
    }
}
