using System;

namespace NKristek.Smaragd
{
    /// <summary>
    /// Defines extension methods for <see cref="WeakReference{T}"/>.
    /// </summary>
    internal static class WeakReferenceExtensions
    {
        /// <summary>
        /// Try to retrieve the target of the given <paramref name="weakReference"/> or get the default value for <typeparamref name="T"/> instead.
        /// </summary>
        /// <typeparam name="T">The generic type of the <see cref="WeakReference{T}"/>.</typeparam>
        /// <param name="weakReference">The <see cref="WeakReference{T}"/> of which the target should be retrieved.</param>
        /// <returns>The target of the given <paramref name="weakReference"/> or the default value for <typeparamref name="T"/>.</returns>
        internal static T TargetOrDefault<T>(this WeakReference<T> weakReference)
            where T : class
        {
            return weakReference != null && weakReference.TryGetTarget(out var target) ? target : default;
        }
    }
}
