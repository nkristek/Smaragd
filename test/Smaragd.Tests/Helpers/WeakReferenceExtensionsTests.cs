using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NKristek.Smaragd.Helpers;
using System.Text;
using Xunit;

namespace NKristek.Smaragd.Tests.Helpers
{
    public class WeakReferenceExtensionsTests
    {
        [Fact]
        public void TargetOrDefault_null_weakreference_throws_ArgumentNullException()
        {
            WeakReference<object> weakReference = null;
            Assert.Throws<ArgumentNullException>(() => weakReference.TargetOrDefault());
        }

        [Fact]
        public void TargetOrDefault_not_disposed_target_returns_target()
        {
            var target = new object();
            var weakReference = new WeakReference<object>(target);
            Assert.Equal(target, weakReference.TargetOrDefault());
        }

        [Fact]
        public void TargetOrDefault_disposed_target_returns_null()
        {
            var weakReference = CreateWeakReferenceToDisposedInstance();
            GCHelper.TriggerGC();
            Assert.Null(weakReference.TargetOrDefault());
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private static WeakReference<object> CreateWeakReferenceToDisposedInstance()
        {
            var target = new object();
            return new WeakReference<object>(target);
        }
    }
}
