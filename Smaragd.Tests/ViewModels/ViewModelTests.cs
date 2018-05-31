using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Tests.ViewModels
{
    /// <summary>
    /// Summary description for ViewModelTests
    /// </summary>
    [TestClass]
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

            private TestChildViewModel _child;
            public TestChildViewModel Child
            {
                get => _child;
                set
                {
                    if (SetProperty(ref _child, value, out var oldValue))
                    {
                        if (oldValue != null)
                            RemoveChildViewModel(oldValue);
                        if (value != null)
                            AddChildViewModel(value);
                    }
                }
            }

            private TestChildViewModel _isDirtyIgnoredChild;

            [IsDirtyIgnored]
            public TestChildViewModel IsDirtyIgnoredChild
            {
                get => _isDirtyIgnoredChild;
                set
                {
                    if (SetProperty(ref _isDirtyIgnoredChild, value, out var oldValue))
                    {
                        if (oldValue != null)
                            RemoveChildViewModel(oldValue);
                        if (value != null)
                            AddChildViewModel(value);
                    }
                }
            }

            private ObservableCollection<int> _values;
            public ObservableCollection<int> Values
            {
                get => _values;
                set => SetProperty(ref _values, value, out _);
            }
        }

        private class TestChildViewModel
            : ViewModel
        {
            private bool _anotherTestProperty;
            public bool AnotherTestProperty
            {
                get => _anotherTestProperty;
                set => SetProperty(ref _anotherTestProperty, value, out _);
            }
        }

        private class TestChildrenViewModel
            : ViewModel
        {
            public TestChildrenViewModel()
            {
                Children.AddCollection(TestChildren);
            }

            public ObservableCollection<TestChildrenViewModel> TestChildren { get; } = new ObservableCollection<TestChildrenViewModel>();
        }

        [TestMethod]
        public void TestChildrenCollection()
        {
            var viewModel = new TestChildrenViewModel();
            var childViewModel = new TestChildrenViewModel();
            viewModel.TestChildren.Add(childViewModel);
            Assert.AreEqual(1, viewModel.Children.Count, "Children count is not correct");
            Assert.AreEqual(viewModel, childViewModel.Parent, "The parent of the child viewmodel was not set");

            viewModel.TestChildren.Clear();
            Assert.AreEqual(null, childViewModel.Parent, "The parent of the child viewmodel was not reset");

            viewModel.TestChildren.Add(childViewModel);
            Assert.AreEqual(1, viewModel.Children.Count, "Children count is not correct");
        }

        [TestMethod]
        public void TestChildren()
        {
            var viewModel = new TestViewModel
            {
                Child = new TestChildViewModel()
            };
            Assert.AreEqual(1, viewModel.Children.Count, "Children count is not correct");
        }

        [TestMethod]
        public void TestCollectionChangedIsDirty()
        {
            var valueCollection = new ObservableCollection<int>();
            var viewModel = new TestViewModel
            {
                Values = valueCollection,
                IsDirty = false
            };
            Assert.IsFalse(viewModel.IsDirty, "ViewModel.IsDirty is true after init");

            valueCollection.Add(1);
            Assert.IsTrue(viewModel.IsDirty, "ViewModel.IsDirty is false after changing the collection");
        }

        [TestMethod]
        public void TestIsDirty()
        {
            var viewModel = new TestViewModel();
            Assert.IsFalse(viewModel.IsDirty, "IsDirty is not initially false");
            viewModel.TestProperty = true;
            Assert.IsTrue(viewModel.IsDirty, "IsDirty wasn't set by the test property");
            
            viewModel.Child = new TestChildViewModel();
            viewModel.IsDirty = false;
            Assert.IsFalse(viewModel.IsDirty, "IsDirty was not reset");
            
            viewModel.Child.AnotherTestProperty = true;
            Assert.IsTrue(viewModel.IsDirty, "IsDirty wasn't set by the child viewmodel property");

            viewModel.IsDirty = false;
            Assert.IsFalse(viewModel.Child.IsDirty, "IsDirty wasn't set on the child viewmodel");
        }

        [TestMethod]
        public void TestIsDirtyIgnored()
        {
            var viewModel = new TestViewModel();
            Assert.IsFalse(viewModel.IsDirty, "IsDirty is not initially false");

            viewModel.IsDirtyIgnoredChild = new TestChildViewModel();
            Assert.IsFalse(viewModel.IsDirty, "IsDirty was set by a property with the IsDirtyIgnoredAttribute set");

            viewModel.IsDirtyIgnoredChild.AnotherTestProperty = true;
            Assert.IsTrue(viewModel.IsDirtyIgnoredChild.IsDirty, "IsDirty wasn't set on the child viewmodel");
            Assert.IsFalse(viewModel.IsDirty, "IsDirty was set but the IsDirtyIgnoredAttribute was set");
        }

        [TestMethod]
        public void TestParent()
        {
            var viewModel = new TestViewModel();
            var childViewModel = new TestChildViewModel
            {
                Parent = viewModel
            };
            Assert.IsNotNull(childViewModel.Parent, "Parent was not set");
            childViewModel.Parent = viewModel;
            Assert.IsNotNull(childViewModel.Parent, "Parent was not set");
        }

        [TestMethod]
        public void TestIsReadonly()
        {
            var viewModel = new TestViewModel
            {
                IsReadOnly = true
            };
            Assert.IsTrue(viewModel.IsReadOnly, "IsReadOnly wasn't set");
            viewModel.TestProperty = true;
            Assert.IsFalse(viewModel.TestProperty, "TestProperty was set although the viewmodel was readonly");
        }

        [TestMethod]
        public void TestChildViewModelPropertyChanged()
        {
            var invokedPropertyChangedEvents = new List<string>();

            var viewModel = new TestViewModel
            {
                Child = new TestChildViewModel()
            };

            viewModel.PropertyChanged += (sender, e) =>
            {
                invokedPropertyChangedEvents.Add(e.PropertyName);
            };

            viewModel.Child.AnotherTestProperty = true;

            Assert.AreEqual(1, invokedPropertyChangedEvents.Count, "Invalid count of invocations of the PropertyChanged event");
            Assert.IsTrue(invokedPropertyChangedEvents.Contains(nameof(TestViewModel.Child)), "The PropertyChanged event wasn't raised for the childviewmodel property");

            viewModel.Child = null;
            Assert.AreEqual(2, invokedPropertyChangedEvents.Count, "Invalid count of invocations of the PropertyChanged event");
        }
    }
}
