using System;
using System.Runtime.CompilerServices;
using Xunit;
using NKristek.Smaragd.Helpers;

namespace NKristek.Smaragd.Tests.Helpers
{
    public class DisposableTests
    {
        private static class DisposableDataStore
        {
            public static bool ManagedRessourcesDisposed { get; set; }
            public static bool NativeRessourcesDisposed { get; set; }
        }

        private class DisposableImpl
            : Disposable
        {
            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();

                Console.WriteLine("Managed");

                DisposableDataStore.ManagedRessourcesDisposed = true;
            }

            protected override void DisposeNativeResources()
            {
                base.DisposeNativeResources();

                Console.WriteLine("Native");

                DisposableDataStore.NativeRessourcesDisposed = true;
            }
        }

        [Fact]
        public void TestDispose()
        {
            DisposableDataStore.ManagedRessourcesDisposed = false;
            DisposableDataStore.NativeRessourcesDisposed = false;

            var instance = new DisposableImpl();
            instance.Dispose();

            Assert.True(DisposableDataStore.ManagedRessourcesDisposed);
            Assert.True(DisposableDataStore.NativeRessourcesDisposed);
        }

        [Fact]
        public void TestFinalize()
        {
            DisposableDataStore.ManagedRessourcesDisposed = false;
            DisposableDataStore.NativeRessourcesDisposed = false;

            CreateInstance();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.False(DisposableDataStore.ManagedRessourcesDisposed);
            Assert.True(DisposableDataStore.NativeRessourcesDisposed);
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private static void CreateInstance()
        {
            var instance = new DisposableImpl();
        }
    }
}
