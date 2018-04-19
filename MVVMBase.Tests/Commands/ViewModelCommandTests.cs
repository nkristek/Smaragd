using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nkristek.MVVMBase.Commands;
using nkristek.MVVMBase.ViewModels;

namespace nkristek.MVVMBase.Tests.Commands
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

        private class TestOnThrownExceptionCommand
            : ViewModelCommand<TestViewModel>
        {
            public TestOnThrownExceptionCommand(TestViewModel parent) : base(parent) { }

            protected override void DoExecute(TestViewModel viewModel, object parameter)
            {
                throw new Exception();
            }

            public bool OnThrownExceptionWasCalled { get; private set; }

            protected override void OnThrownException(TestViewModel viewModel, object parameter, Exception exception)
            {
                OnThrownExceptionWasCalled = true;
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
        public void TestOnThrownException()
        {
            var viewModel = new TestViewModel();
            var command = new TestOnThrownExceptionCommand(viewModel);
            command.Execute(viewModel);
            Assert.IsTrue(command.OnThrownExceptionWasCalled, "OnThrownException was not called");
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
