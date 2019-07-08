using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.ViewModels;
using Xunit;

namespace NKristek.Smaragd.Tests.ViewModels
{
    public class ViewModelTests
    {
        private class TestViewModel
            : ViewModel
        {
            private object _property;

            public object Property
            {
                get => _property;
                set => SetProperty(ref _property, value);
            }

            private WeakReference<object> _weakProperty;

            public object WeakProperty
            {
                get => _weakProperty?.TargetOrDefault();
                set => SetProperty(ref _weakProperty, value);
            }

            private object _isDirtyIgnoredProperty;

            [IsDirtyIgnored]
            public object IsDirtyIgnoredProperty
            {
                get => _isDirtyIgnoredProperty;
                set => SetProperty(ref _isDirtyIgnoredProperty, value);
            }

            private WeakReference<object> _isDirtyIgnoredWeakProperty;

            [IsDirtyIgnored]
            public object IsDirtyIgnoredWeakProperty
            {
                get => _isDirtyIgnoredWeakProperty?.TargetOrDefault();
                set => SetProperty(ref _isDirtyIgnoredWeakProperty, value);
            }

            private object _isReadOnlyIgnoredProperty;

            [IsReadOnlyIgnored]
            public object IsReadOnlyIgnoredProperty
            {
                get => _isReadOnlyIgnoredProperty;
                set => SetProperty(ref _isReadOnlyIgnoredProperty, value);
            }

            private WeakReference<object> _isReadOnlyIgnoredWeakProperty;

            [IsReadOnlyIgnored]
            public object IsReadOnlyIgnoredWeakProperty
            {
                get => _isReadOnlyIgnoredWeakProperty?.TargetOrDefault();
                set => SetProperty(ref _isReadOnlyIgnoredWeakProperty, value);
            }

            private ObservableCollection<int> _values = new ObservableCollection<int>();

            public ObservableCollection<int> Values
            {
                get => _values;
                set => SetProperty(ref _values, value);
            }

            private WeakReference<ObservableCollection<int>> _weakValues;

            public ObservableCollection<int> WeakValues
            {
                get => _weakValues?.TargetOrDefault();
                set => SetProperty(ref _weakValues, value);
            }

            private ObservableCollection<int> _isDirtyIgnoredValues = new ObservableCollection<int>();

            [IsDirtyIgnored]
            public ObservableCollection<int> IsDirtyIgnoredValues
            {
                get => _isDirtyIgnoredValues;
                set => SetProperty(ref _isDirtyIgnoredValues, value);
            }

            private WeakReference<ObservableCollection<int>> _isDirtyIgnoredWeakValues;

            [IsDirtyIgnored]
            public ObservableCollection<int> IsDirtyIgnoredWeakValues
            {
                get => _isDirtyIgnoredWeakValues?.TargetOrDefault();
                set => SetProperty(ref _isDirtyIgnoredWeakValues, value);
            }
        }

        #region IsDirty

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void IsDirty_set(bool initialValue, bool valueToSet)
        {
            var viewModel = new TestViewModel
            {
                IsDirty = initialValue
            };
            Assert.Equal(initialValue, viewModel.IsDirty);
            viewModel.IsDirty = valueToSet;
            Assert.Equal(valueToSet, viewModel.IsDirty);
        }

        [Fact]
        public void IsDirty_is_initially_false()
        {
            var viewModel = new TestViewModel();
            Assert.False(viewModel.IsDirty);
        }

        [Fact]
        public void Property_sets_IsDirty()
        {
            var value = new object();
            var viewModel = new TestViewModel
            {
                Property = value
            };
            Assert.True(viewModel.IsDirty);
        }

        [Fact]
        public void WeakProperty_sets_IsDirty()
        {
            var value = new object();
            var viewModel = new TestViewModel
            {
                WeakProperty = value
            };
            Assert.True(viewModel.IsDirty);
        }

        [Fact]
        public void Property_with_IsDirtyIgnored_does_not_set_IsDirty()
        {
            var value = new object();
            var viewModel = new TestViewModel
            {
                IsDirtyIgnoredProperty = value
            };
            Assert.False(viewModel.IsDirty);
        }

        [Fact]
        public void WeakProperty_with_IsDirtyIgnored_does_not_set_IsDirty()
        {
            var value = new object();
            var viewModel = new TestViewModel
            {
                IsDirtyIgnoredWeakProperty = value
            };
            Assert.False(viewModel.IsDirty);
        }

        private class InheritIsDirtyIgnoredBase
            : ViewModel
        {
            private bool _property;

            public virtual bool Property
            {
                get => _property;
                set => SetProperty(ref _property, value, out _);
            }

            private bool _isDirtyIgnoredProperty;

            [IsDirtyIgnored]
            public virtual bool IsDirtyIgnoredProperty
            {
                get => _isDirtyIgnoredProperty;
                set => SetProperty(ref _isDirtyIgnoredProperty, value, out _);
            }
        }

        private class InheritIsDirtyIgnoredDerived
            : InheritIsDirtyIgnoredBase
        {
            [IsDirtyIgnored(InheritAttributes = true)]
            public override bool Property
            {
                get => base.Property;
                set => base.Property = value;
            }

            [IsDirtyIgnored(InheritAttributes = true)]
            public override bool IsDirtyIgnoredProperty
            {
                get => base.IsDirtyIgnoredProperty;
                set => base.IsDirtyIgnoredProperty = value;
            }
        }

        [Fact]
        public void IsDirtyIgnoredAttribute_InheritAttributes()
        {
            var viewModel = new InheritIsDirtyIgnoredDerived
            {
                IsDirtyIgnoredProperty = true
            };
            Assert.False(viewModel.IsDirty);
            viewModel.Property = true;
            Assert.True(viewModel.IsDirty);
        }

        [Fact]
        public void Initial_collection_sets_IsDirty()
        {
            var viewModel = new TestViewModel();
            viewModel.Values.Add(1);
            Assert.True(viewModel.IsDirty);
        }

        [Fact]
        public void New_collection_sets_IsDirty()
        {
            var oldCollection = new ObservableCollection<int>();
            var viewModel = new TestViewModel
            {
                Values = oldCollection,
                IsDirty = false
            };
            var newCollection = new ObservableCollection<int>();
            viewModel.Values = newCollection;
            viewModel.IsDirty = false;
            newCollection.Add(1);
            Assert.True(viewModel.IsDirty);
        }

        [Fact]
        public void New_weak_collection_sets_IsDirty()
        {
            var oldCollection = new ObservableCollection<int>();
            var viewModel = new TestViewModel
            {
                WeakValues = oldCollection,
                IsDirty = false
            };
            var newCollection = new ObservableCollection<int>();
            viewModel.WeakValues = newCollection;
            viewModel.IsDirty = false;
            newCollection.Add(1);
            Assert.True(viewModel.IsDirty);
        }

        [Fact]
        public void Initial_collection_with_IsDirtyIgnored_does_not_set_IsDirty()
        {
            var viewModel = new TestViewModel();
            viewModel.IsDirtyIgnoredValues.Add(1);
            Assert.False(viewModel.IsDirty);
        }

        [Fact]
        public void New_collection_with_IsDirtyIgnored_does_not_set_IsDirty()
        {
            var oldCollection = new ObservableCollection<int>();
            var viewModel = new TestViewModel
            {
                IsDirtyIgnoredValues = oldCollection,
                IsDirty = false
            };
            var newCollection = new ObservableCollection<int>();
            viewModel.IsDirtyIgnoredValues = newCollection;
            viewModel.IsDirty = false;
            newCollection.Add(1);
            Assert.False(viewModel.IsDirty);
        }

        [Fact]
        public void New_weak_collection_with_IsDirtyIgnored_does_not_set_IsDirty()
        {
            var oldCollection = new ObservableCollection<int>();
            var viewModel = new TestViewModel
            {
                IsDirtyIgnoredWeakValues = oldCollection,
                IsDirty = false
            };
            var newCollection = new ObservableCollection<int>();
            viewModel.IsDirtyIgnoredWeakValues = newCollection;
            viewModel.IsDirty = false;
            newCollection.Add(1);
            Assert.False(viewModel.IsDirty);
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
        public void IsReadOnly_does_not_set_IsDirty()
        {
            var viewModel = new TestViewModel
            {
                IsReadOnly = true
            };
            Assert.False(viewModel.IsDirty);
        }

        [Fact]
        public void IsUpdating_does_not_set_IsDirty()
        {
            var viewModel = new TestViewModel
            {
                IsUpdating = true
            };
            Assert.False(viewModel.IsDirty);
        }

        #endregion

        #region Parent

        public static IEnumerable<object[]> Parent_set_input
        {
            get
            {
                yield return new object[] { null, new TestViewModel() };
                yield return new object[] { new TestViewModel(), null };
            }
        }

        [Theory]
        [MemberData(nameof(Parent_set_input))]
        public void Parent_set(ViewModel initialValue, ViewModel valueToSet)
        {
            var viewModel = new TestViewModel
            {
                Parent = initialValue
            };
            Assert.Equal(initialValue, viewModel.Parent);
            viewModel.Parent = valueToSet;
            Assert.Equal(valueToSet, viewModel.Parent);
        }

        [Fact]
        public void Parent_is_initially_null()
        {
            var viewModel = new TestViewModel();
            Assert.Null(viewModel.Parent);
        }

        [Fact]
        public void Parent_is_weak_referenced()
        {
            var viewModel = new TestViewModel();
            SetParent(viewModel);
            GCHelper.TriggerGC();
            Assert.Null(viewModel.Parent);
        }
        
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private static void SetParent(ViewModel viewModel)
        {
            viewModel.Parent = new TestViewModel();
        }

        #endregion

        #region IsReadOnly

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void IsReadOnly_set(bool initialValue, bool valueToSet)
        {
            var viewModel = new TestViewModel
            {
                IsReadOnly = initialValue
            };
            Assert.Equal(initialValue, viewModel.IsReadOnly);
            viewModel.IsReadOnly = valueToSet;
            Assert.Equal(valueToSet, viewModel.IsReadOnly);
        }

        [Fact]
        public void IsReadOnly_is_initially_false()
        {
            var viewModel = new TestViewModel();
            Assert.False(viewModel.IsReadOnly);
        }

        [Fact]
        public void Property_can_not_be_set_when_IsReadOnly()
        {
            var value = new object();
            var viewModel = new TestViewModel
            {
                IsReadOnly = true,
                Property = value
            };
            Assert.Null(viewModel.Property);
        }

        [Fact]
        public void WeakProperty_can_not_be_set_when_IsReadOnly()
        {
            var value = new object();
            var viewModel = new TestViewModel
            {
                IsReadOnly = true,
                WeakProperty = value
            };
            Assert.Null(viewModel.WeakProperty);
        }

        [Fact]
        public void Property_with_IsReadOnlyIgnored_can_be_set_when_IsReadOnly()
        {
            var value = new object();
            var viewModel = new TestViewModel
            {
                IsReadOnly = true,
                IsReadOnlyIgnoredProperty = value
            };
            Assert.NotNull(viewModel.IsReadOnlyIgnoredProperty);
        }

        [Fact]
        public void WeakProperty_with_IsReadOnlyIgnored_can_be_set_when_IsReadOnly()
        {
            var value = new object();
            var viewModel = new TestViewModel
            {
                IsReadOnly = true,
                IsReadOnlyIgnoredWeakProperty = value
            };
            Assert.NotNull(viewModel.IsReadOnlyIgnoredWeakProperty);
        }

        private class InheritIsReadOnlyIgnoredBase
            : ViewModel
        {
            private bool _property;

            public virtual bool Property
            {
                get => _property;
                set => SetProperty(ref _property, value, out _);
            }

            private bool _isReadOnlyIgnoredProperty;

            [IsReadOnlyIgnored]
            public virtual bool IsReadOnlyIgnoredProperty
            {
                get => _isReadOnlyIgnoredProperty;
                set => SetProperty(ref _isReadOnlyIgnoredProperty, value, out _);
            }
        }

        private class InheritIsReadOnlyIgnoredDerived
            : InheritIsReadOnlyIgnoredBase
        {
            [IsReadOnlyIgnored(InheritAttributes = true)]
            public override bool Property
            {
                get => base.Property;
                set => base.Property = value;
            }

            [IsReadOnlyIgnored(InheritAttributes = true)]
            public override bool IsReadOnlyIgnoredProperty
            {
                get => base.IsReadOnlyIgnoredProperty;
                set => base.IsReadOnlyIgnoredProperty = value;
            }
        }

        [Fact]
        public void IsReadOnlyIgnoredAttribute_InheritAttributes()
        {
            var viewModel = new InheritIsReadOnlyIgnoredDerived
            {
                IsReadOnly = true,
                Property = true,
                IsReadOnlyIgnoredProperty = true
            };
            Assert.False(viewModel.Property);
            Assert.True(viewModel.IsReadOnlyIgnoredProperty);
        }

        [Fact]
        public void IsDirty_can_be_set_when_IsReadOnly()
        {
            var viewModel = new TestViewModel
            {
                IsDirty = true,
                IsReadOnly = true
            };
            viewModel.IsDirty = false;
            Assert.False(viewModel.IsDirty);
        }

        [Fact]
        public void Parent_can_be_set_when_IsReadOnly()
        {
            var parent = new TestViewModel();
            var viewModel = new TestViewModel
            {
                IsReadOnly = true,
                Parent = parent
            };
            Assert.NotNull(viewModel.Parent);
        }

        [Fact]
        public void IsUpdating_can_be_set_when_IsReadOnly()
        {
            var viewModel = new TestViewModel
            {
                IsReadOnly = true,
                IsUpdating = true
            };
            Assert.True(viewModel.IsUpdating);
        }

        #endregion

        #region IsUpdating

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void IsUpdating_set(bool initialValue, bool valueToSet)
        {
            var viewModel = new TestViewModel
            {
                IsUpdating = initialValue
            };
            Assert.Equal(initialValue, viewModel.IsUpdating);
            viewModel.IsUpdating = valueToSet;
            Assert.Equal(valueToSet, viewModel.IsUpdating);
        }

        [Fact]
        public void IsUpdating_initially_false()
        {
            var viewModel = new TestViewModel();
            Assert.False(viewModel.IsUpdating);
        }

        #endregion

        #region PropertySource

        private class PropertySourceViewModel
            : ViewModel
        {
            private bool _testProperty;

            public bool TestProperty
            {
                get => _testProperty;
                set => SetProperty(ref _testProperty, value);
            }

            [PropertySource(nameof(TestProperty))]
            public bool AnotherTestProperty => TestProperty;

            [PropertySource("NotExistingProperty")]
            public bool TestPropertyWithNonExistentSource { get; }

            [PropertySource(nameof(PropertySourceLoopSecondProperty))]
            public bool PropertySourceLoopFirstProperty { get; }

            [PropertySource(nameof(PropertySourceLoopFirstProperty))]
            public bool PropertySourceLoopSecondProperty { get; }

            public void NotifyPropertyChangedExternal(string propertyName)
            {
                NotifyPropertyChanged(propertyName);
            }
        }

        [Fact]
        public void PropertySourceAttribute_raises_event_on_PropertyChanged()
        {
            var invokedPropertyChangedEvents = new List<string>();
            var viewModel = new PropertySourceViewModel();
            viewModel.PropertyChanged += (sender, e) => invokedPropertyChangedEvents.Add(e.PropertyName);
            viewModel.TestProperty = true;
            var expectedPropertyChangedEvents = new List<string>
            {
                nameof(PropertySourceViewModel.IsDirty),
                nameof(PropertySourceViewModel.TestProperty),
                nameof(PropertySourceViewModel.AnotherTestProperty)
            };
            Assert.Equal(expectedPropertyChangedEvents, invokedPropertyChangedEvents);
        }

        [Fact]
        public void Looping_PropertySourceAttributes_get_resolved()
        {
            var invokedPropertyChangedEvents = new List<string>();
            var viewModel = new PropertySourceViewModel();
            viewModel.PropertyChanged += (sender, e) => invokedPropertyChangedEvents.Add(e.PropertyName);
            viewModel.NotifyPropertyChangedExternal(nameof(PropertySourceViewModel.PropertySourceLoopFirstProperty));
            var expectedPropertyChangedEvents = new List<string>
            {
                nameof(PropertySourceViewModel.IsDirty),
                nameof(PropertySourceViewModel.PropertySourceLoopFirstProperty),
                nameof(PropertySourceViewModel.PropertySourceLoopSecondProperty)
            };
            Assert.Equal(expectedPropertyChangedEvents, invokedPropertyChangedEvents);
        }

        private class InheritPropertySourceBase
            : ViewModel
        {
            private bool _testProperty;

            public bool TestProperty
            {
                get => _testProperty;
                set => SetProperty(ref _testProperty, value, out _);
            }

            [PropertySource(nameof(TestProperty))]
            public virtual bool SecondTestProperty => TestProperty;

            [PropertySource(nameof(TestProperty))]
            public virtual bool ThirdTestProperty => TestProperty;
        }

        private class InheritPropertySourceDerived
            : InheritPropertySourceBase
        {
            [PropertySource(InheritAttributes = true)]
            public override bool SecondTestProperty => base.SecondTestProperty;

            public override bool ThirdTestProperty => true;
        }

        [Fact]
        public void PropertySourceAttribute_InheritAttributes()
        {
            var invokedPropertyChangedEvents = new List<string>();
            var viewModel = new InheritPropertySourceDerived();
            viewModel.PropertyChanged += (sender, e) => invokedPropertyChangedEvents.Add(e.PropertyName);
            viewModel.TestProperty = true;
            var expectedPropertyChangedEvents = new List<string>
            {
                nameof(InheritPropertySourceDerived.IsDirty),
                nameof(InheritPropertySourceDerived.TestProperty),
                nameof(InheritPropertySourceDerived.SecondTestProperty)
            };
            Assert.Equal(expectedPropertyChangedEvents, invokedPropertyChangedEvents);
        }

        #endregion
    }
}