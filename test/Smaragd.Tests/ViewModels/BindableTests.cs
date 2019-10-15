using System;
using System.Collections.Generic;
using System.Linq;
using NKristek.Smaragd.ViewModels;
using NKristek.Smaragd.Helpers;
using Xunit;

namespace NKristek.Smaragd.Tests.ViewModels
{
    public class BindableTests
    {
        private class TestBindable
            : Bindable
        {
            public object? PropertyStorage;

            public object? Property
            {
                get => PropertyStorage;
                set => SetProperty(ref PropertyStorage, value);
            }

            public WeakReference<object>? WeakPropertyStorage;

            public object? WeakProperty
            {
                get => WeakPropertyStorage?.TargetOrDefault();
                set => SetProperty(ref WeakPropertyStorage, value);
            }

            public string? StringPropertyStorage;

            public string? StringProperty
            {
                get => StringPropertyStorage;
                set => SetProperty(ref StringPropertyStorage, value);
            }

            public WeakReference<string>? WeakStringPropertyStorage;

            public string? WeakStringProperty
            {
                get => WeakStringPropertyStorage?.TargetOrDefault();
                set => SetProperty(ref WeakStringPropertyStorage, value);
            }

            public void NotifyPropertyChangingExternal(string? propertyName)
            {
                NotifyPropertyChanging(propertyName);
            }

            public void NotifyPropertyChangedExternal(string? propertyName)
            {
                NotifyPropertyChanged(propertyName);
            }

            public bool SetPropertyExternal<T>(ref T storage, T value, IEqualityComparer<T>? comparer, string? propertyName)
            {
                return base.SetProperty(ref storage, value, comparer, propertyName);
            }

            public bool SetPropertyExternal<T>(ref T storage, T value, out T oldValue, IEqualityComparer<T>? comparer, string? propertyName)
            {
                return base.SetProperty(ref storage, value, out oldValue, comparer, propertyName);
            }

            public bool SetPropertyExternal<T>(ref WeakReference<T>? storage, T? value, IEqualityComparer<T?>? comparer, string? propertyName)
                where T : class
            {
                return base.SetProperty(ref storage, value, comparer, propertyName);
            }

            public bool SetPropertyExternal<T>(ref WeakReference<T>? storage, T? value, out T? oldValue, IEqualityComparer<T?>? comparer, string? propertyName)
                where T : class
            {
                return base.SetProperty(ref storage, value, out oldValue, comparer, propertyName);
            }
        }

        private class StringSameLengthEqualityComparer
            : IEqualityComparer<string?>
        {
            /// <inheritdoc />
            public bool Equals(string? x, string? y)
            {
                return (x?.Length ?? 0) == (y?.Length ?? 0);
            }

            /// <inheritdoc />
            public int GetHashCode(string? obj)
            {
                return obj?.Length ?? 0;
            }
        }

        [Theory]
        [InlineData(nameof(TestBindable.Property))]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void NotifyPropertyChanging_raises_event_on_PropertyChanging(string? propertyName)
        {
            var invokedPropertyChangingEvents = new List<string>();
            var bindable = new TestBindable();
            bindable.PropertyChanging += (sender, args) => invokedPropertyChangingEvents.Add(args.PropertyName);
            bindable.NotifyPropertyChangingExternal(propertyName);
            Assert.Equal(Enumerable.Repeat(propertyName, 1), invokedPropertyChangingEvents);
        }

        [Theory]
        [InlineData(nameof(TestBindable.Property))]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void NotifyPropertyChanged_raises_event_on_PropertyChanged(string? propertyName)
        {
            var invokedPropertyChangedEvents = new List<string>();
            var bindable = new TestBindable();
            bindable.PropertyChanged += (sender, args) => invokedPropertyChangedEvents.Add(args.PropertyName);
            bindable.NotifyPropertyChangedExternal(propertyName);
            Assert.Equal(Enumerable.Repeat(propertyName, 1), invokedPropertyChangedEvents);
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(null)]
        public void SetProperty_sets_storage(object? input)
        {
            var bindable = new TestBindable();
            bindable.SetPropertyExternal(ref bindable.PropertyStorage, input, null, nameof(bindable.Property));
            Assert.Equal(input, bindable.PropertyStorage);
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(null)]
        public void SetProperty_weak_sets_storage(object? input)
        {
            var bindable = new TestBindable();
            bindable.SetPropertyExternal(ref bindable.WeakPropertyStorage, input, null, nameof(bindable.WeakProperty));
            Assert.Equal(input, bindable.WeakPropertyStorage?.TargetOrDefault());
        }

        [Theory]
        [InlineData(null, typeof(object))]
        [InlineData(typeof(object), null)]
        public void SetProperty_sets_old_value_of_storage(object? initialValue, object? input)
        {
            var bindable = new TestBindable
            {
                Property = initialValue
            };
            var expectedOldValue = bindable.PropertyStorage;
            bindable.SetPropertyExternal(ref bindable.PropertyStorage, input, out var oldValue, null, nameof(bindable.Property));
            Assert.Equal(expectedOldValue, oldValue);
        }

        [Theory]
        [InlineData(null, typeof(object))]
        [InlineData(typeof(object), null)]
        public void SetProperty_weak_sets_old_value_of_storage(object? initialValue, object? input)
        {
            var bindable = new TestBindable
            {
                WeakProperty = initialValue
            };
            var expectedOldValue = bindable.WeakPropertyStorage?.TargetOrDefault();
            bindable.SetPropertyExternal(ref bindable.WeakPropertyStorage, input, out var oldValue, null, nameof(bindable.WeakProperty));
            Assert.Equal(expectedOldValue, oldValue);
        }

        [Fact]
        public void SetProperty_uses_callermembername_as_propertyname()
        {
            var bindable = new TestBindable();
            var newValue = new object();
            var invokedPropertyChangingEvents = new List<string>();
            var invokedPropertyChangedEvents = new List<string>();
            bindable.PropertyChanging += (sender, args) => invokedPropertyChangingEvents.Add(args.PropertyName);
            bindable.PropertyChanged += (sender, args) => invokedPropertyChangedEvents.Add(args.PropertyName);
            bindable.Property = newValue;
            Assert.Contains(nameof(bindable.Property), invokedPropertyChangingEvents);
            Assert.Single(invokedPropertyChangingEvents);
            Assert.Contains(nameof(bindable.Property), invokedPropertyChangedEvents);
            Assert.Single(invokedPropertyChangedEvents);
        }

        [Fact]
        public void SetProperty_weak_uses_callermembername_as_propertyname()
        {
            var bindable = new TestBindable();
            var newValue = new object();
            var invokedPropertyChangingEvents = new List<string>();
            var invokedPropertyChangedEvents = new List<string>();
            bindable.PropertyChanging += (sender, args) => invokedPropertyChangingEvents.Add(args.PropertyName);
            bindable.PropertyChanged += (sender, args) => invokedPropertyChangedEvents.Add(args.PropertyName);
            bindable.WeakProperty = newValue;
            Assert.Contains(nameof(bindable.WeakProperty), invokedPropertyChangingEvents);
            Assert.Single(invokedPropertyChangingEvents);
            Assert.Contains(nameof(bindable.WeakProperty), invokedPropertyChangedEvents);
            Assert.Single(invokedPropertyChangedEvents);
        }

        [Theory]
        [InlineData(typeof(object), 1)]
        [InlineData(null, 0)]
        public void SetProperty_raises_event_on_PropertyChanging(object? input, int expectedCountOfPropertyChangingEvents)
        {
            var invokedPropertyChangingEvents = new List<string>();
            var bindable = new TestBindable();
            bindable.PropertyChanging += (sender, args) => invokedPropertyChangingEvents.Add(args.PropertyName);
            bindable.Property = input;
            Assert.Equal(Enumerable.Repeat(nameof(bindable.Property), expectedCountOfPropertyChangingEvents), invokedPropertyChangingEvents);
        }

        [Theory]
        [InlineData(typeof(object), 1)]
        [InlineData(null, 0)]
        public void SetProperty_weak_raises_event_on_PropertyChanging(object? input, int expectedCountOfPropertyChangingEvents)
        {
            var invokedPropertyChangingEvents = new List<string>();
            var bindable = new TestBindable();
            bindable.PropertyChanging += (sender, args) => invokedPropertyChangingEvents.Add(args.PropertyName);
            bindable.WeakProperty = input;
            Assert.Equal(Enumerable.Repeat(nameof(bindable.WeakProperty), expectedCountOfPropertyChangingEvents), invokedPropertyChangingEvents);
        }

        [Theory]
        [InlineData(typeof(object), 1)]
        [InlineData(null, 0)]
        public void SetProperty_raises_event_on_PropertyChanged(object? input, int expectedCountOfPropertyChangedEvents)
        {
            var invokedPropertyChangedEvents = new List<string>();
            var bindable = new TestBindable();
            bindable.PropertyChanged += (sender, args) => invokedPropertyChangedEvents.Add(args.PropertyName);
            bindable.Property = input;
            Assert.Equal(Enumerable.Repeat(nameof(bindable.Property), expectedCountOfPropertyChangedEvents), invokedPropertyChangedEvents);
        }

        [Theory]
        [InlineData(typeof(object), 1)]
        [InlineData(null, 0)]
        public void SetProperty_weak_raises_event_on_PropertyChanged(object? input, int expectedCountOfPropertyChangedEvents)
        {
            var invokedPropertyChangedEvents = new List<string>();
            var bindable = new TestBindable();
            bindable.PropertyChanged += (sender, args) => invokedPropertyChangedEvents.Add(args.PropertyName);
            bindable.WeakProperty = input;
            Assert.Equal(Enumerable.Repeat(nameof(bindable.WeakProperty), expectedCountOfPropertyChangedEvents), invokedPropertyChangedEvents);
        }

        [Fact]
        public void SetProperty_raises_event_on_PropertyChanging_before_storage_changed()
        {
            var bindable = new TestBindable();
            var oldValue = bindable.Property;
            var newValue = new object();
            bindable.PropertyChanging += (sender, args) => Assert.Equal(oldValue, bindable.Property);
            bindable.Property = newValue;
        }

        [Fact]
        public void SetProperty_weak_raises_event_on_PropertyChanging_before_storage_changed()
        {
            var bindable = new TestBindable();
            var oldValue = bindable.WeakProperty;
            var newValue = new object();
            bindable.PropertyChanging += (sender, args) => Assert.Equal(oldValue, bindable.WeakProperty);
            bindable.WeakProperty = newValue;
        }

        [Fact]
        public void SetProperty_raises_event_on_PropertyChanged_after_storage_changed()
        {
            var bindable = new TestBindable();
            var newValue = new object();
            bindable.PropertyChanged += (sender, args) => Assert.Equal(newValue, bindable.Property);
            bindable.Property = newValue;
        }

        [Fact]
        public void SetProperty_weak_raises_event_on_PropertyChanged_after_storage_changed()
        {
            var bindable = new TestBindable();
            var newValue = new object();
            bindable.PropertyChanged += (sender, args) => Assert.Equal(newValue, bindable.WeakProperty);
            bindable.WeakProperty = newValue;
        }

        [Theory]
        [InlineData(null, typeof(object), true)]
        [InlineData(null, null, false)]
        public void SetProperty_returns_if_value_was_different(object? initialValue, object? input, bool expectedResult)
        {
            var bindable = new TestBindable
            {
                Property = initialValue
            };
            Assert.Equal(expectedResult, bindable.SetPropertyExternal(ref bindable.PropertyStorage, input, null, nameof(bindable.Property)));
        }

        [Theory]
        [InlineData(null, typeof(object), true)]
        [InlineData(null, null, false)]
        public void SetProperty_weak_returns_if_value_was_different(object? initialValue, object? input, bool expectedResult)
        {
            var bindable = new TestBindable
            {
                Property = initialValue
            };
            Assert.Equal(expectedResult, bindable.SetPropertyExternal(ref bindable.PropertyStorage, input, null, nameof(bindable.Property)));
        }
        
        [Theory]
        [InlineData("a", "a", false)]
        [InlineData("a", "b", false)]
        [InlineData("a", "aa", true)]
        public void SetProperty_uses_comparer(string initialValue, string input, bool expectedResult)
        {
            var bindable = new TestBindable
            {
                StringProperty = initialValue
            };
            var comparer = new StringSameLengthEqualityComparer();
            Assert.Equal(expectedResult, bindable.SetPropertyExternal(ref bindable.StringPropertyStorage, input, comparer, nameof(bindable.StringProperty)));
        }

        [Theory]
        [InlineData("a", "a", false)]
        [InlineData("a", "b", false)]
        [InlineData("a", "aa", true)]
        public void SetProperty_weak_uses_comparer(string initialValue, string input, bool expectedResult)
        {
            var bindable = new TestBindable
            {
                WeakStringProperty = initialValue
            };
            var comparer = new StringSameLengthEqualityComparer();
            Assert.Equal(expectedResult, bindable.SetPropertyExternal(ref bindable.WeakStringPropertyStorage, input, comparer, nameof(bindable.WeakStringProperty)));
        }
    }
}