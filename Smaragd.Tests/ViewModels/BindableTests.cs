using System;
using System.Collections.Generic;
using System.Linq;
using NKristek.Smaragd.ViewModels;
using Xunit;

namespace NKristek.Smaragd.Tests.ViewModels
{
    public class BindableTests
    {
        private class BindableTest 
            : Bindable
        {
            public bool TestPropertyStorage;

            public bool TestProperty
            {
                get => TestPropertyStorage;
                set => SetProperty(ref TestPropertyStorage, value, out var oldValue);
            }
            
            public void RaisePropertyChangedExternal(string propertyName = null)
            {
                base.RaisePropertyChanged(propertyName);
            }

            public bool SetPropertyExternal<T>(ref T storage, T value, out T oldValue, string propertyName = "")
            {
                return SetProperty(ref storage, value, out oldValue, propertyName);
            }
        }

        [Fact]
        public void RaisePropertyChanged_raises_event_on_PropertyChanged()
        {
            const string propertyName = nameof(BindableTest.TestProperty);
            var invokedPropertyChangedEvents = new List<string>();
            var bindable = new BindableTest();
            bindable.PropertyChanged += (sender, args) => invokedPropertyChangedEvents.Add(args.PropertyName);
            bindable.RaisePropertyChangedExternal(propertyName);
            Assert.Equal(Enumerable.Repeat(propertyName, 1), invokedPropertyChangedEvents);
        }

        [Fact]
        public void RaisePropertyChanged_PropertyNameEmpty_ThrowsArgumentNullException()
        {
            var bindable = new BindableTest();
            Assert.Throws<ArgumentNullException>(() => bindable.RaisePropertyChangedExternal(""));
        }

        [Fact]
        public void RaisePropertyChanged_PropertyNameWhitespace_ThrowsArgumentNullException()
        {
            var bindable = new BindableTest();
            Assert.Throws<ArgumentNullException>(() => bindable.RaisePropertyChangedExternal(" "));
        }

        [Fact]
        public void RaisePropertyChanged_PropertyNameNull_ThrowsArgumentNullException()
        {
            var bindable = new BindableTest();
            Assert.Throws<ArgumentNullException>(() => bindable.RaisePropertyChangedExternal(null));
        }

        [Fact]
        public void SetProperty_PropertyNameEmpty_ThrowsArgumentNullException()
        {
            var bindable = new BindableTest();
            Assert.Throws<ArgumentNullException>(() => bindable.SetPropertyExternal(ref bindable.TestPropertyStorage, true, out _, ""));
        }

        [Fact]
        public void SetProperty_PropertyNameWhitespace_ThrowsArgumentNullException()
        {
            var bindable = new BindableTest();
            Assert.Throws<ArgumentNullException>(() => bindable.SetPropertyExternal(ref bindable.TestPropertyStorage, true, out _, " "));
        }

        [Fact]
        public void SetProperty_PropertyNameNull_ThrowsArgumentNullException()
        {
            var bindable = new BindableTest();
            Assert.Throws<ArgumentNullException>(() => bindable.SetPropertyExternal(ref bindable.TestPropertyStorage, true, out _, null));
        }

        [Fact]
        public void SetProperty_returns_old_value()
        {
            var bindable = new BindableTest();
            bindable.SetPropertyExternal(ref bindable.TestPropertyStorage, true, out var oldValue, nameof(bindable.TestProperty));
            Assert.False(oldValue);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void SetProperty_sets_storage_value(bool input, bool expectedResult)
        {
            var bindable = new BindableTest
            {
                TestProperty = input
            };
            Assert.Equal(expectedResult, bindable.TestProperty);
        }

        [Theory]
        [InlineData(false, true, true)]
        [InlineData(false, false, false)]
        public void SetProperty_returns_if_value_was_different(bool initialValue, bool input, bool expectedResult)
        {
            var bindable = new BindableTest
            {
                TestProperty = initialValue
            };
            Assert.Equal(expectedResult, bindable.SetPropertyExternal(ref bindable.TestPropertyStorage, input, out _, nameof(bindable.TestProperty)));
        }

        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void SetProperty_raises_event_on_PropertyChanged(bool input, int expectedCountOfPropertyChangedEvents)
        {
            var invokedPropertyChangedEvents = new List<string>();
            var bindable = new BindableTest();
            bindable.PropertyChanged += (sender, args) => invokedPropertyChangedEvents.Add(args.PropertyName);
            bindable.TestProperty = input;
            Assert.Equal(Enumerable.Repeat(nameof(bindable.TestProperty), expectedCountOfPropertyChangedEvents), invokedPropertyChangedEvents);
        }
    }
}
