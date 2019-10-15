using System;
using System.Collections.Generic;
using System.Linq;
using NKristek.Smaragd.Helpers;
using Xunit;

namespace NKristek.Smaragd.Tests.Helpers
{
    public class NotificationCacheTests
    {
        private INotificationCache NotificationCache { get; }

        private string FirstProperty { get; }

        private string SecondProperty { get; }

        private string ThirdProperty { get; }

        public NotificationCacheTests()
        {
            NotificationCache = new NotificationCache();
            FirstProperty = nameof(FirstProperty);
            SecondProperty = nameof(SecondProperty);
            ThirdProperty = nameof(ThirdProperty);
        }

        [Fact]
        public void AddPropertyNameToNotify_PropertyNameOfNotifyingPropertyNull_ThrowsArgumentNullException()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(() => NotificationCache.AddPropertyNameToNotify(null, FirstProperty));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void AddPropertyNameToNotify_PropertyNameToNotifyNull_ThrowsArgumentNullException()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(() => NotificationCache.AddPropertyNameToNotify(FirstProperty, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void AddPropertyNameToNotify_SameName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => NotificationCache.AddPropertyNameToNotify(FirstProperty, FirstProperty));
        }

        [Fact]
        public void GetPropertyNamesToNotify_propertyName_null_throws_ArgumentNullException()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.Throws<ArgumentNullException>(() => NotificationCache.GetPropertyNamesToNotify(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void GetPropertyNamesToNotify()
        {
            var notificationCache = new NotificationCache();
            notificationCache.AddPropertyNameToNotify(FirstProperty, SecondProperty);
            var expectedPropertyNames = Enumerable.Repeat(SecondProperty, 1);
            var propertyNames = notificationCache.GetPropertyNamesToNotify(FirstProperty);
            Assert.Equal(expectedPropertyNames, propertyNames);
        }

        [Fact]
        public void GetPropertyNamesToNotify_IndirectPropertyNames()
        {
            var notificationCache = new NotificationCache();
            notificationCache.AddPropertyNameToNotify(FirstProperty, SecondProperty);
            notificationCache.AddPropertyNameToNotify(SecondProperty, ThirdProperty);
            var expectedPropertyNames = new List<string> {SecondProperty, ThirdProperty}.OrderBy(name => name);
            var propertyNames = notificationCache.GetPropertyNamesToNotify(FirstProperty).OrderBy(name => name);
            Assert.Equal(expectedPropertyNames, propertyNames);
        }

        [Fact]
        public void GetPropertyNamesToNotify_RecursivePropertyNames()
        {
            var notificationCache = new NotificationCache();
            notificationCache.AddPropertyNameToNotify(FirstProperty, SecondProperty);
            notificationCache.AddPropertyNameToNotify(SecondProperty, FirstProperty);
            var expectedPropertyNames = Enumerable.Repeat(SecondProperty, 1);
            var propertyNames = notificationCache.GetPropertyNamesToNotify(FirstProperty);
            Assert.Equal(expectedPropertyNames, propertyNames);
        }
    }
}