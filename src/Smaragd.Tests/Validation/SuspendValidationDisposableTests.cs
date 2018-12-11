using System;
using NKristek.Smaragd.Validation;
using NKristek.Smaragd.ViewModels;
using Xunit;

namespace NKristek.Smaragd.Tests.Validation
{
    public class SuspendValidationDisposableTests
    {
        private class TestViewModel : ValidatingViewModel
        {
        }

        [Fact]
        public void SuspendValidationDisposable_ViewModelNullThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new SuspendValidationDisposable(null));
        }

        [Fact]
        public void OnCreate_SetValidationSuspended()
        {
            var viewModel = new TestViewModel();
            using (new SuspendValidationDisposable(viewModel))
                Assert.True(viewModel.ValidationSuspended);
        }

        [Fact]
        public void OnDisposed_UnsetValidationSuspended()
        {
            var viewModel = new TestViewModel();
            var suspendValidationDisposable = new SuspendValidationDisposable(viewModel);
            suspendValidationDisposable.Dispose();
            Assert.False(viewModel.ValidationSuspended);
        }
    }
}