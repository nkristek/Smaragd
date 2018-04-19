using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nkristek.MVVMBase.Commands;
using nkristek.MVVMBase.ViewModels;

namespace nkristek.MVVMBase.Tests.Commands
{
    /// <summary>
    /// Summary description for AsyncViewModelCommandTests
    /// </summary>
    [TestClass]
    public class AsyncViewModelCommandTests
    {
        private class TestViewModel
            : ViewModel
        {

        }

        private class TestCommand
            : AsyncViewModelCommand<TestViewModel>
        {
            public TestCommand(TestViewModel parent) : base(parent) { }

            protected override async Task DoExecute(TestViewModel viewModel, object parameter)
            {
                await Task.Run(() =>
                {
                    if (viewModel == null)
                        throw new ArgumentNullException(nameof(viewModel));

                    if (parameter == null)
                        throw new ArgumentNullException(nameof(parameter));

                    if (viewModel != parameter)
                        throw new Exception("invalid parameter");
                });
            }
        }

        private class TestOnThrownExceptionCommand
            : AsyncViewModelCommand<TestViewModel>
        {
            public TestOnThrownExceptionCommand(TestViewModel parent) : base(parent) { }

            protected override async Task DoExecute(TestViewModel viewModel, object parameter)
            {
                await Task.Run(() => throw new Exception());
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
            command.ExecuteAsync(viewModel).Wait();
        }

        [TestMethod]
        public void TestOnThrownException()
        {
            var viewModel = new TestViewModel();
            var command = new TestOnThrownExceptionCommand(viewModel);
            command.ExecuteAsync(viewModel).Wait();
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
