using System;
using System.Collections.Generic;

namespace nkristek.MVVMBase.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Executes an action on each element of the enumeration
        /// </summary>
        /// <typeparam name="T">Type of the elements of the enumeration</typeparam>
        /// <param name="enumeration">The enumeration on which the action should get executed</param>
        /// <param name="action">The action which gets executed</param>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (var item in enumeration)
                action(item);
        }
    }
}
