using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Tests.Commands
{
    /// <summary>
    /// Summary description for ViewModelCommandTests
    /// </summary>
    [TestClass]
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

        [TestMethod]
        public void TestViewModelCommand()
        {
            var viewModel = new TestViewModel();
            var command = new TestCommand(viewModel);
            command.Execute(viewModel);
        }

        [TestMethod]
        public void TestParent()
        {
            var viewModel = new TestViewModel();
            var command = new TestCommand(viewModel);
            Assert.IsNotNull(command.Parent, "The command parent is null");
        }

        [TestMethod]
        public void TestNoParentException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TestCommand(null), "ViewModelCommand did not throw an exception when instancing with no parent");
        }
    }
}
