using Microsoft.VisualStudio.TestTools.UnitTesting;
using nkristek.MVVMBase;
using System;
using System.Runtime.CompilerServices;

namespace nkristek.MVVMBaseTest
{
    [TestClass]
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

        [TestMethod]
        public void TestDispose()
        {
            DisposableDataStore.ManagedRessourcesDisposed = false;
            DisposableDataStore.NativeRessourcesDisposed = false;

            var instance = new DisposableImpl();
            instance.Dispose();

            Assert.IsTrue(DisposableDataStore.ManagedRessourcesDisposed);
            Assert.IsTrue(DisposableDataStore.NativeRessourcesDisposed);
        }

        [TestMethod]
        public void TestFinalize()
        {
            DisposableDataStore.ManagedRessourcesDisposed = false;
            DisposableDataStore.NativeRessourcesDisposed = false;

            CreateInstance();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.IsFalse(DisposableDataStore.ManagedRessourcesDisposed);
            Assert.IsTrue(DisposableDataStore.NativeRessourcesDisposed);
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private void CreateInstance()
        {
            var instance = new DisposableImpl();
        }
    }
}
