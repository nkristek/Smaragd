using NKristek.Smaragd.Helpers;
using Xunit;

namespace NKristek.Smaragd.Tests.Helpers
{
    public class ActionDisposableTests
    {
        [Fact]
        public void Constructor_no_actions_no_exceptions()
        {
            var instance = new ActionDisposable(null);
            instance.Dispose();
        }

        [Fact]
        public void DisposeManagedResources_Dispose()
        {
            var managedResourcesDisposed = false;
            var instance = new ActionDisposable(() => managedResourcesDisposed = true);
            instance.Dispose();
            Assert.True(managedResourcesDisposed);
        }

        [Fact]
        public void DisposeNativeResourcesDisposed_Dispose()
        {
            var nativeResourcesDisposed = false;
            var instance = new ActionDisposable(null, () => nativeResourcesDisposed = true);
            instance.Dispose();
            Assert.True(nativeResourcesDisposed);
        }
    }
}
