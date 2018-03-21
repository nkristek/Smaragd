using System;
using System.Collections.Generic;

namespace nkristek.MVVMBase.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Executes an <see cref="Action{T}"/> on each element of the <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="T">Type of the elements of the <see cref="IEnumerable{T}"/></typeparam>
        /// <param name="enumeration">The <see cref="IEnumerable{T}"/> on which the <see cref="Action{T}"/> should get executed</param>
        /// <param name="action">The <see cref="Action{T}"/> which gets executed on each element of the <see cref="IEnumerable{T}"/></param>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (var item in enumeration)
                action(item);
        }
    }
}
