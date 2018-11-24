using System.Collections.ObjectModel;
using System.Linq;
using Xunit;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Tests.ViewModels
{
    public class ViewModelTests
    {
        private class TestViewModel
            : ViewModel
        {
            private bool _testProperty;

            public bool TestProperty
            {
                get => _testProperty;
                set => SetProperty(ref _testProperty, value, out _);
            }
        }

        private class TestIsDirtyViewModel
            : ViewModel
        {
            private bool _testProperty;

            public bool TestProperty
            {
                get => _testProperty;
                set => SetProperty(ref _testProperty, value, out _);
            }

            private bool _isDirtyIgnoredProperty;

            [IsDirtyIgnored]
            public bool IsDirtyIgnoredProperty
            {
                get => _isDirtyIgnoredProperty;
                set => SetProperty(ref _isDirtyIgnoredProperty, value, out _);
            }
        }

        private class TestIsDirtyCollectionViewModel
            : ViewModel
        {
            private ObservableCollection<int> _values = new ObservableCollection<int>();

            public ObservableCollection<int> Values
            {
                get => _values;
                set => SetProperty(ref _values, value, out _);
            }

            private ObservableCollection<int> _isDirtyIgnoredValues = new ObservableCollection<int>();

            [IsDirtyIgnored]
            public ObservableCollection<int> IsDirtyIgnoredValues
            {
                get => _isDirtyIgnoredValues;
                set => SetProperty(ref _isDirtyIgnoredValues, value, out _);
            }
        }

        [Fact]
        public void TestIsDirty()
        {
            var viewModel = new TestIsDirtyViewModel();
            Assert.False(viewModel.IsDirty, "IsDirty is initially true");
            Assert.False(viewModel.TestProperty, "TestProperty is initially true");
            Assert.False(viewModel.IsDirtyIgnoredProperty, "IsDirtyIgnoredProperty is initially true");

            viewModel.TestProperty = true;
            Assert.True(viewModel.IsDirty, "IsDirty wasn't set by the test property");

            viewModel.IsDirty = false;
            Assert.False(viewModel.IsDirty, "IsDirty was not set to false");

            viewModel.IsDirtyIgnoredProperty = true;
            Assert.False(viewModel.IsDirty, "IsDirtyIgnoredProperty was not ignored");
        }

        [Fact]
        public void TestIsDirtyCollection()
        {
            var viewModel = new TestIsDirtyCollectionViewModel();
            Assert.False(viewModel.IsDirty, "IsDirty is initially true");
            Assert.False(viewModel.Values.Any(), "Values has initially items");
            Assert.False(viewModel.IsDirtyIgnoredValues.Any(), "IsDirtyIgnoredValues has initially items");

            viewModel.Values.Add(1);
            Assert.True(viewModel.IsDirty, "IsDirty wasn't set by changing the collection");

            viewModel.IsDirty = false;
            Assert.False(viewModel.IsDirty, "IsDirty was not set to false");

            viewModel.IsDirtyIgnoredValues.Add(1);
            Assert.False(viewModel.IsDirty, "IsDirtyIgnoredValues was not ignored");
        }

        [Fact]
        public void TestParent()
        {
            var viewModel = new TestViewModel();
            var childViewModel = new TestViewModel
            {
                Parent = viewModel
            };
            Assert.NotNull(childViewModel.Parent);
        }

        [Fact]
        public void TestIsReadonly()
        {
            var viewModel = new TestViewModel
            {
                IsReadOnly = true
            };
            Assert.True(viewModel.IsReadOnly, "IsReadOnly wasn't set");

            viewModel.TestProperty = true;
            Assert.False(viewModel.TestProperty, "TestProperty was set although the viewmodel was readonly");
        }
    }
}
