using System;
using System.Linq;
using Xunit;
using NKristek.Smaragd.Helpers;

namespace NKristek.Smaragd.Tests.Helpers
{
    public class NotificationCacheTests
    {
        [Fact]
        public void TestNotificationCache()
        {
            var notifyingProperty = "NotifyingProperty";
            var propertyToNotify = "PropertyToNotify";

            var notificationCache = new NotificationCache();
            Assert.False(notificationCache.GetPropertyNamesToNotify(notifyingProperty).Any(), "The NotificationCache returned names although there are none added yet.");

            notificationCache.AddPropertyNameToNotify(notifyingProperty, propertyToNotify);
            Assert.True(notificationCache.GetPropertyNamesToNotify(notifyingProperty).Contains(propertyToNotify), "The NotificationCache did not return the property to notify.");
            
            Assert.False(notificationCache.GetPropertyNamesToNotify("SomeOtherProperty").Any(), "The NotificationCache returned names although there should be none for a property which wasn't added.");
        }

        [Fact]
        public void TestNestedPropertySourcesAndLoops()
        {
            var firstProperty = "FirstProperty";
            var secondProperty = "SecondProperty";
            var thirdProperty = "ThirdProperty";

            var notificationCache = new NotificationCache();
            notificationCache.AddPropertyNameToNotify(firstProperty, secondProperty);
            notificationCache.AddPropertyNameToNotify(secondProperty, thirdProperty);
            notificationCache.AddPropertyNameToNotify(thirdProperty, firstProperty);

            var propertyNamesToNotify = notificationCache.GetPropertyNamesToNotify(firstProperty).ToList();
            Assert.True(propertyNamesToNotify.Count == 2, "The count of property names is not 2.");
            Assert.True(propertyNamesToNotify.Contains(secondProperty), "The property names to notify did not contain SecondProperty");
            Assert.True(propertyNamesToNotify.Contains(thirdProperty), "The property names to notify did not contain ThirdProperty");
        }

        [Fact]
        public void TestExceptions()
        {
            var notificationCache = new NotificationCache();

            Assert.Throws<ArgumentNullException>(() => notificationCache.AddPropertyNameToNotify(null, "not null"));
            Assert.Throws<ArgumentNullException>(() => notificationCache.AddPropertyNameToNotify("not null", null));
            Assert.Throws<ArgumentException>(() => notificationCache.AddPropertyNameToNotify("The same", "The same"));

            Assert.Throws<ArgumentNullException>(() => notificationCache.GetPropertyNamesToNotify(null));
        }
    }
}
