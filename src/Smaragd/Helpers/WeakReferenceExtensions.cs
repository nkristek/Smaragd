using System;

namespace NKristek.Smaragd.Helpers
{
    /// <summary>
    /// Defines extension methods for <see cref="WeakReference{T}"/>.
    /// </summary>
    public static class WeakReferenceExtensions
    {
        /// <summary>
        /// Try to retrieve the target of the given <paramref name="weakReference"/> or get the <see langword="default"/> value of <typeparamref name="T"/> instead.
        /// </summary>
        /// <typeparam name="T">The generic type of the <see cref="WeakReference{T}"/>.</typeparam>
        /// <param name="weakReference">The <see cref="WeakReference{T}"/> of which the target should be retrieved.</param>
        /// <returns>The target of the given <paramref name="weakReference"/> or the <see langword="default"/> value of <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="weakReference"/> is <see langword="null"/>.</exception>
        public static T? TargetOrDefault<T>(this WeakReference<T> weakReference)
            where T : class
        {
            if (weakReference == null)
                throw new ArgumentNullException(nameof(weakReference));
            return weakReference.TryGetTarget(out var target) ? target : default;
        }
    }
}
