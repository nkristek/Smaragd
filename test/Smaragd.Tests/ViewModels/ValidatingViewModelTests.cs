using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using NKristek.Smaragd.ViewModels;
using Xunit;

namespace NKristek.Smaragd.Tests.ViewModels
{
    public class ValidatingViewModelTests
    {
        private class TestViewModel
            : ValidatingViewModel
        {
            private int _property;

            public int Property
            {
                get => _property;
                set => SetProperty(ref _property, value);
            }

            private int _anotherProperty;

            public int AnotherProperty
            {
                get => _anotherProperty;
                set => SetProperty(ref _anotherProperty, value);
            }

            public void NotifyPropertyChangingExternal(string propertyName)
            {
                NotifyPropertyChanging(propertyName);
            }

            public void NotifyPropertyChangedExternal(string propertyName)
            {
                NotifyPropertyChanged(propertyName);
            }

            public bool HasNotifiedErrorsChanged;

            protected override void NotifyErrorsChanged([CallerMemberName] string propertyName = null)
            {
                base.NotifyErrorsChanged(propertyName);
                HasNotifiedErrorsChanged = true;
            }

            public void NotifyErrorsChangedExternal(string propertyName)
            {
                NotifyErrorsChanged(propertyName);
            }
        }

        #region IsValid

        [Fact]
        public void IsValid_is_false_when_validation_errors_exist()
        {
            var viewModel = new TestViewModel();
            viewModel.SetErrors(Enumerable.Repeat("error", 1), nameof(viewModel.Property));
            Assert.False(viewModel.IsValid);
        }

        [Fact]
        public void IsValid_is_true_when_no_validation_errors_exist()
        {
            var viewModel = new TestViewModel();
            viewModel.SetErrors(Enumerable.Repeat("error", 1), nameof(viewModel.Property));
            viewModel.SetErrors(Enumerable.Empty<string>(), nameof(viewModel.Property));
            Assert.True(viewModel.IsValid);
        }

        [Fact]
        public void IsValid_does_not_set_IsDirty()
        {
            var viewModel = new TestViewModel();
            viewModel.NotifyPropertyChangedExternal(nameof(viewModel.IsValid));
            Assert.False(viewModel.IsDirty);
        }

        [Fact]
        public void IsValid_gets_notified_by_HasErrors()
        {
            var invokedPropertyChangingEvents = new List<string>();
            var invokedPropertyChangedEvents = new List<string>();
            var viewModel = new TestViewModel();
            viewModel.PropertyChanging += (sender, args) => invokedPropertyChangingEvents.Add(args.PropertyName);
            viewModel.PropertyChanged += (sender, args) => invokedPropertyChangedEvents.Add(args.PropertyName);

            viewModel.NotifyPropertyChangingExternal(nameof(viewModel.HasErrors));
            viewModel.NotifyPropertyChangedExternal(nameof(viewModel.HasErrors));

            Assert.Contains(nameof(viewModel.IsValid), invokedPropertyChangingEvents);
            Assert.Contains(nameof(viewModel.IsValid), invokedPropertyChangedEvents);
        }

        #endregion

        #region SetErrors

        [Fact]
        public void SetErrors_throws_ArgumentNullException_when_propertyName_is_null()
        {
            var viewModel = new TestViewModel();
            Assert.Throws<ArgumentNullException>(() => viewModel.SetErrors(Enumerable.Empty<string>(), null));
        }

        [Fact]
        public void SetErrors_propertyName_is_set_by_CallerMemberName()
        {
            var invokedErrorChangedEvents = new List<string>();
            var viewModel = new TestViewModel();
            viewModel.ErrorsChanged += (sender, args) => invokedErrorChangedEvents.Add(args.PropertyName);
            viewModel.SetErrors(Enumerable.Repeat("error", 1));
            Assert.Contains(nameof(SetErrors_propertyName_is_set_by_CallerMemberName), invokedErrorChangedEvents);
        }

        [Fact]
        public void SetErrors_sets_errors_of_property()
        {
            var errors = Enumerable.Repeat("error", 1);
            var viewModel = new TestViewModel();
            viewModel.SetErrors(errors, nameof(viewModel.Property));
            Assert.Equal(errors, viewModel.GetErrors(nameof(viewModel.Property)));
        }

        [Fact]
        public void SetErrors_null_removes_errors_of_property()
        {
            var errors = Enumerable.Repeat("error", 1);
            var viewModel = new TestViewModel();
            viewModel.SetErrors(errors, nameof(viewModel.Property));
            viewModel.SetErrors(null, nameof(viewModel.Property));
            Assert.Empty(viewModel.GetErrors(nameof(viewModel.Property)));
        }

        [Fact]
        public void SetErrors_empty_collection_removes_errors_of_property()
        {
            var errors = Enumerable.Repeat("error", 1);
            var viewModel = new TestViewModel();
            viewModel.SetErrors(errors, nameof(viewModel.Property));
            viewModel.SetErrors(Enumerable.Empty<string>(), nameof(viewModel.Property));
            Assert.Empty(viewModel.GetErrors(nameof(viewModel.Property)));
        }

        [Fact]
        public void SetErrors_notifies_when_errors_changed()
        {
            var viewModel = new TestViewModel();
            Assert.False(viewModel.HasNotifiedErrorsChanged);
            viewModel.SetErrors(Enumerable.Repeat("error", 1));
            Assert.True(viewModel.HasNotifiedErrorsChanged);
        }

        #endregion

        #region HasErrors

        [Fact]
        public void HasErrors_is_true_when_validation_errors_exist()
        {
            var viewModel = new TestViewModel();
            viewModel.SetErrors(Enumerable.Repeat("error", 1), nameof(viewModel.Property));
            Assert.True(viewModel.HasErrors);
        }

        [Fact]
        public void HasErrors_is_false_when_no_validation_errors_exist()
        {
            var viewModel = new TestViewModel();
            viewModel.SetErrors(Enumerable.Repeat("error", 1), nameof(viewModel.Property));
            viewModel.SetErrors(Enumerable.Empty<string>(), nameof(viewModel.Property));
            Assert.False(viewModel.HasErrors);
        }

        [Fact]
        public void HasErrors_does_not_set_IsDirty()
        {
            var viewModel = new TestViewModel();
            viewModel.NotifyPropertyChangedExternal(nameof(viewModel.HasErrors));
            Assert.False(viewModel.IsDirty);
        }

        [Fact]
        public void HasErrors_gets_notified_before_errors_change()
        {
            var invokedPropertyChangingEvents = new List<string>();
            var viewModel = new TestViewModel();
            viewModel.PropertyChanging += (sender, args) => invokedPropertyChangingEvents.Add(args.PropertyName);
            viewModel.SetErrors(Enumerable.Repeat("error", 1), nameof(viewModel.Property));
            Assert.Contains(nameof(viewModel.HasErrors), invokedPropertyChangingEvents);
        }

        [Fact]
        public void HasErrors_gets_notified_after_errors_change()
        {
            var invokedPropertyChangedEvents = new List<string>();
            var viewModel = new TestViewModel();
            viewModel.PropertyChanged += (sender, args) => invokedPropertyChangedEvents.Add(args.PropertyName);
            viewModel.SetErrors(Enumerable.Repeat("error", 1), nameof(viewModel.Property));
            Assert.Contains(nameof(viewModel.HasErrors), invokedPropertyChangedEvents);
        }

        #endregion

        #region GetErrors

        [Theory]
        [InlineData(null, 2)]
        [InlineData("", 2)]
        [InlineData(nameof(TestViewModel.Property), 1)]
        [InlineData("NotExistingProperty", 0)]
        public void GetErrors_with_error(string propertyName, int expectedErrorCount)
        {
            var viewModel = new TestViewModel();
            viewModel.SetErrors(Enumerable.Repeat("Value has to be at least 5.", 1), nameof(TestViewModel.Property));
            viewModel.SetErrors(Enumerable.Repeat("Value has to be at least 5.", 1), nameof(TestViewModel.AnotherProperty));
            Assert.Equal(Enumerable.Repeat("Value has to be at least 5.", expectedErrorCount), viewModel.GetErrors(propertyName));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(nameof(TestViewModel.Property))]
        [InlineData("NotExistingProperty")]
        public void GetErrors_without_error(string propertyName)
        {
            var viewModel = new TestViewModel();
            viewModel.SetErrors(Enumerable.Repeat("Value has to be at least 5.", 1), nameof(TestViewModel.Property));
            viewModel.SetErrors(Enumerable.Empty<string>(), nameof(viewModel.Property));
            Assert.Empty(viewModel.GetErrors(propertyName));
        }

        [Theory]
        [InlineData(null, 2)]
        [InlineData("", 2)]
        [InlineData(nameof(TestViewModel.Property), 1)]
        [InlineData("NotExistingProperty", 0)]
        public void INotifyDataErrorInfoGetErrors_with_error(string propertyName, int expectedErrorCount)
        {
            var viewModel = new TestViewModel();
            viewModel.SetErrors(Enumerable.Repeat("Value has to be at least 5.", 1), nameof(TestViewModel.Property));
            viewModel.SetErrors(Enumerable.Repeat("Value has to be at least 5.", 1), nameof(TestViewModel.AnotherProperty));
            Assert.Equal(Enumerable.Repeat("Value has to be at least 5.", expectedErrorCount), ((INotifyDataErrorInfo)viewModel).GetErrors(propertyName));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(nameof(TestViewModel.Property))]
        [InlineData("NotExistingProperty")]
        public void INotifyDataErrorInfoGetErrors_without_error(string propertyName)
        {
            var viewModel = new TestViewModel();
            viewModel.SetErrors(Enumerable.Repeat("Value has to be at least 5.", 1), nameof(TestViewModel.Property));
            viewModel.SetErrors(Enumerable.Empty<string>(), nameof(viewModel.Property));
            Assert.Empty(((INotifyDataErrorInfo)viewModel).GetErrors(propertyName));
        }

        #endregion

        #region NotifyErrorsChanged

        [Fact]
        public void NotifyErrorsChanged_raises_event_on_ErrorsChanged()
        {
            var invokedErrorChangedEvents = new List<string>();
            var viewModel = new TestViewModel();
            viewModel.ErrorsChanged += (sender, args) => invokedErrorChangedEvents.Add(args.PropertyName);

            viewModel.NotifyErrorsChangedExternal(nameof(viewModel.Property));

            Assert.Equal(Enumerable.Repeat(nameof(viewModel.Property), 1), invokedErrorChangedEvents);
        }

        #endregion
    }
}