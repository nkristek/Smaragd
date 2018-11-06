using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NKristek.Smaragd.Helpers;

namespace NKristek.Smaragd.Tests.Helpers
{
    [TestClass]
    public class NotificationCacheTests
    {
        [TestMethod]
        public void TestNotificationCache()
        {
            var notifyingProperty = "NotifyingProperty";
            var propertyToNotify = "PropertyToNotify";

            var notificationCache = new NotificationCache();
            Assert.IsFalse(notificationCache.GetPropertyNamesToNotify(notifyingProperty).Any(), "The NotificationCache returned names although there are none added yet.");

            notificationCache.AddPropertyNameToNotify(notifyingProperty, propertyToNotify);
            Assert.IsTrue(notificationCache.GetPropertyNamesToNotify(notifyingProperty).Contains(propertyToNotify), "The NotificationCache did not return the property to notify.");
            
            Assert.IsFalse(notificationCache.GetPropertyNamesToNotify("SomeOtherProperty").Any(), "The NotificationCache returned names although there should be none for a property which wasn't added.");
        }

        [TestMethod]
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
            Assert.IsTrue(propertyNamesToNotify.Count == 2, "The count of property names is not 2.");
            Assert.IsTrue(propertyNamesToNotify.Contains(secondProperty), "The property names to notify did not contain SecondProperty");
            Assert.IsTrue(propertyNamesToNotify.Contains(thirdProperty), "The property names to notify did not contain ThirdProperty");
        }

        [TestMethod]
        public void TestExceptions()
        {
            var notificationCache = new NotificationCache();

            Assert.ThrowsException<ArgumentNullException>(() => notificationCache.AddPropertyNameToNotify(null, "not null"), "The first parameter of AddPropertyNameToNotify() should not be null.");
            Assert.ThrowsException<ArgumentNullException>(() => notificationCache.AddPropertyNameToNotify("not null", null), "The second parameter of AddPropertyNameToNotify() should not be null.");
            Assert.ThrowsException<ArgumentException>(() => notificationCache.AddPropertyNameToNotify("The same", "The same"), "The parameters of AddPropertyNameToNotify() should not be the same.");

            Assert.ThrowsException<ArgumentNullException>(() => notificationCache.GetPropertyNamesToNotify(null), "The parameter of GetPropertyNamesToNotify() should not be null.");
        }
    }
}
