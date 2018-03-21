using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// This method merges two sequences.
        /// It works like <see cref="IEnumerable{T}.Union(IEnumerable{T}).Distinct()"/>, but also tries to preserve the order of the secondary sequence.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="primarySequence">Primary sequence which will be prioritzed when conflicts occur</param>
        /// <param name="secondarySequence">Secondary sequence</param>
        /// <param name="equalsPropertyAccessor">This function should return a property which is used to differentiate between the <see cref="T"/> instances.</param>
        /// <returns></returns>
        public static IEnumerable<T> MergeSequence<T>(this IEnumerable<T> primarySequence, IEnumerable<T> secondarySequence, Func<T, object> equalsPropertyAccessor = null)
        {
            if (primarySequence == null)
                throw new ArgumentNullException("primarySequence");
            if (secondarySequence == null)
                throw new ArgumentNullException("secondarySequence");
            if (equalsPropertyAccessor == null)
                equalsPropertyAccessor = arg => arg;

            var primaryItems = primarySequence.ToList();
            var notContainedItems = new List<T>();

            foreach (var secondaryItem in secondarySequence)
            {
                var secondaryItemProperty = equalsPropertyAccessor(secondaryItem);
                var indexOfPrimaryItem = primaryItems.FindIndex(item =>
                {
                    var itemProperty = equalsPropertyAccessor(item);
                    if (itemProperty != null)
                        return itemProperty.Equals(secondaryItemProperty);
                    return secondaryItemProperty == null;
                });

                var secondaryItemExistsInPrimaryItems = indexOfPrimaryItem >= 0;
                if (secondaryItemExistsInPrimaryItems)
                {
                    // insert notContainedItems before this item
                    primaryItems.InsertRange(indexOfPrimaryItem, notContainedItems);
                    notContainedItems.Clear();
                }
                else
                {
                    notContainedItems.Add(secondaryItem);
                }
            }
            primaryItems.AddRange(notContainedItems);

            return primaryItems;
        }
    }
}
