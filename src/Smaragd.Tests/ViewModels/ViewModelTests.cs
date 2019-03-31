using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.ViewModels;
using Xunit;

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

        private class TestViewModelCommand
            : ViewModelCommand<TestViewModel>
        {
            /// <inheritdoc />
            [CanExecuteSource(nameof(TestViewModel.TestProperty))]
            protected override bool CanExecute(TestViewModel viewModel, object parameter)
            {
                return viewModel.TestProperty;
            }

            /// <inheritdoc />
            protected override void Execute(TestViewModel viewModel, object parameter)
            {
                throw new NotImplementedException();
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

            private ObservableCollection<int> _privateValues = new ObservableCollection<int>();

            private ObservableCollection<int> PrivateValues
            {
                get => _privateValues;
                set => SetProperty(ref _privateValues, value, out _);
            }

            public void AddValueToPrivateValues(int value)
            {
                PrivateValues.Add(value);
            }
        }

        [Fact]
        public void IsDirty_initially_false()
        {
            var viewModel = new TestIsDirtyViewModel();
            Assert.False(viewModel.IsDirty);
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void IsDirty_set(bool initialValue, bool valueToSet)
        {
            var viewModel = new TestIsDirtyViewModel
            {
                IsDirty = initialValue
            };
            viewModel.IsDirty = valueToSet;
            Assert.Equal(valueToSet, viewModel.IsDirty);
        }

        [Fact]
        public void SetProperty_IsDirty()
        {
            var viewModel = new TestIsDirtyViewModel
            {
                TestProperty = true
            };
            Assert.True(viewModel.IsDirty);
        }

        [Fact]
        public void SetProperty_IsDirty_IsDirtyIgnoredAttribute()
        {
            var viewModel = new TestIsDirtyViewModel
            {
                IsDirtyIgnoredProperty = true
            };
            Assert.False(viewModel.IsDirty);
        }

        private class InheritIsDirtyIgnoredParent
            : ViewModel
        {
            private bool _testProperty;

            public virtual bool TestProperty
            {
                get => _testProperty;
                set => SetProperty(ref _testProperty, value, out _);
            }

            private bool _isDirtyIgnoredProperty;

            [IsDirtyIgnored]
            public virtual bool IsDirtyIgnoredProperty
            {
                get => _isDirtyIgnoredProperty;
                set => SetProperty(ref _isDirtyIgnoredProperty, value, out _);
            }
        }

        private class InheritIsDirtyIgnoredChild
            : InheritIsDirtyIgnoredParent
        {
            private bool _testProperty;

            [IsDirtyIgnored(InheritAttributes = true)]
            public override bool TestProperty
            {
                get => _testProperty;
                set => SetProperty(ref _testProperty, value, out _);
            }

            private bool _isDirtyIgnoredProperty;

            [IsDirtyIgnored(InheritAttributes = true)]
            public override bool IsDirtyIgnoredProperty
            {
                get => _isDirtyIgnoredProperty;
                set => SetProperty(ref _isDirtyIgnoredProperty, value, out _);
            }
        }

        [Fact]
        public void SetProperty_IsDirty_IsDirtyIgnoredAttribute_InheritAttributes()
        {
            var viewModel = new InheritIsDirtyIgnoredChild
            {
                IsDirtyIgnoredProperty = true
            };
            Assert.False(viewModel.IsDirty);
            viewModel.TestProperty = true;
            Assert.True(viewModel.IsDirty);
        }

        [Fact]
        public void IsDirty_from_collection()
        {
            var viewModel = new TestIsDirtyCollectionViewModel();
            viewModel.Values.Add(1);
            Assert.True(viewModel.IsDirty);
        }

        [Fact]
        public void IsDirty_from_new_collection()
        {
            var viewModel = new TestIsDirtyCollectionViewModel
            {
                Values = new ObservableCollection<int>(),
                IsDirty = false
            };
            viewModel.Values.Add(1);
            Assert.True(viewModel.IsDirty);
        }

        [Fact]
        public void IsDirty_from_collection_IsDirtyIgnoredAttribute()
        {
            var viewModel = new TestIsDirtyCollectionViewModel();
            viewModel.IsDirtyIgnoredValues.Add(1);
            Assert.False(viewModel.IsDirty);
        }

        [Fact]
        public void IsDirty_from_new_collection_IsDirtyIgnoredAttribute()
        {
            var viewModel = new TestIsDirtyCollectionViewModel
            {
                IsDirtyIgnoredValues = new ObservableCollection<int>()
            };
            viewModel.IsDirtyIgnoredValues.Add(1);
            Assert.False(viewModel.IsDirty);
        }

        [Fact]
        public void IsDirty_from_private_collection()
        {
            var viewModel = new TestIsDirtyCollectionViewModel();
            viewModel.AddValueToPrivateValues(1);
            Assert.False(viewModel.IsDirty);
        }

        [Fact]
        public void Parent()
        {
            var viewModel = new TestViewModel();
            var childViewModel = new TestViewModel
            {
                Parent = viewModel
            };
            Assert.Equal(viewModel, childViewModel.Parent);
        }

        [Fact]
        public void Parent_disposed()
        {
            var viewModel = new TestViewModel();
            SetDisposingParent(viewModel);
            GCHelper.TriggerGC();
            Assert.Null(viewModel.Parent);
        }
        
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private static void SetDisposingParent(ViewModel viewModel)
        {
            viewModel.Parent = new TestViewModel();
        }

        [Fact]
        public void Parent_does_not_set_IsDirty()
        {
            var parentViewModel = new TestViewModel();
            var viewModel = new TestViewModel
            {
                Parent = parentViewModel
            };
            Assert.False(viewModel.IsDirty);
        }

        [Fact]
        public void Parent_set_to_null()
        {
            var parentViewModel = new TestViewModel();
            var viewModel = new TestViewModel
            {
                Parent = parentViewModel
            };

            viewModel.Parent = null;
            Assert.Null(viewModel.Parent);
        }

        [Fact]
        public void Parent_set_no_change_doesnt_raise_PropertyChanged()
        {
            var parentViewModel = new TestViewModel();
            var viewModel = new TestViewModel
            {
                Parent = parentViewModel
            };

            var propertyChangedInvocations = 0;
            viewModel.PropertyChanged += (sender, args) => propertyChangedInvocations++;
            viewModel.Parent = parentViewModel;
            Assert.Equal(0, propertyChangedInvocations);
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void IsReadOnly_set(bool initialValue, bool valueToSet)
        {
            var viewModel = new TestViewModel
            {
                IsReadOnly = initialValue
            };
            viewModel.IsReadOnly = valueToSet;
            Assert.Equal(valueToSet, viewModel.IsReadOnly);
        }

        [Fact]
        public void IsReadOnly_does_not_set_IsDirty()
        {
            var viewModel = new TestViewModel
            {
                IsReadOnly = true
            };
            Assert.False(viewModel.IsDirty);
        }

        [Fact]
        public void SetProperty_IsReadonly()
        {
            var viewModel = new TestViewModel
            {
                IsReadOnly = true,
                TestProperty = true
            };
            Assert.False(viewModel.TestProperty);
        }

        [Fact]
        public void Commands_not_null()
        {
            var viewModel = new TestViewModel();
            Assert.NotNull(viewModel.Commands);
        }

        [Fact]
        public void AddCommand()
        {
            var viewModel = new TestViewModel();
            var command = new TestViewModelCommand
            {
                Parent = viewModel
            };
            viewModel.AddCommand(command);
            Assert.True(viewModel.Commands.ContainsKey(command.Name));
            Assert.Equal(command, viewModel.Commands[command.Name]);
        }

        [Fact]
        public void RemoveCommand()
        {
            var viewModel = new TestViewModel();
            var command = new TestViewModelCommand
            {
                Parent = viewModel
            };
            viewModel.AddCommand(command);
            Assert.True(viewModel.RemoveCommand(command));
            Assert.False(viewModel.Commands.ContainsKey(command.Name));
        }

        [Fact]
        public void IsDirty_can_be_set_when_IsReadOnly()
        {
            var viewModel = new TestViewModel
            {
                TestProperty = true,
                IsReadOnly = true,
                IsDirty = false
            };
            Assert.False(viewModel.IsDirty);
        }
    }
}