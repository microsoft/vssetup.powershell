// <copyright file="Extensions.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup.PowerShell
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;

    /// <summary>
    /// Extension methods.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Determines if the results of the <paramref name="selector"/> applied to the <paramref name="source"/> enumerable
        /// contain all the elements from the <paramref name="keys"/>.
        /// </summary>
        /// <typeparam name="T">The type of the source enumerable.</typeparam>
        /// <typeparam name="TKey">The type of the keys.</typeparam>
        /// <param name="source">The source enumerable to check.</param>
        /// <param name="selector">The selector to apply to the <paramref name="source"/> enumerable.</param>
        /// <param name="keys">The keys to check if all are in the <paramref name="source"/> enumerable.</param>
        /// <param name="comparer">The comparer to use. The default is <see cref="EqualityComparer{T}.Default"/>.</param>
        /// <returns>
        /// True if the results of the <paramref name="selector"/> applied to the <paramref name="source"/> enumerable
        /// contain all the elements from the <paramref name="keys"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">One or more parameters are null.</exception>
        public static bool ContainsAll<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, IEnumerable<TKey> keys, IEqualityComparer<TKey> comparer = null)
        {
            Validate.NotNull(source, nameof(source));
            Validate.NotNull(selector, nameof(selector));
            Validate.NotNull(keys, nameof(keys));

            comparer = comparer ?? EqualityComparer<TKey>.Default;

            var e = source.Select(selector);
            var items = new HashSet<TKey>(e, comparer);

            foreach (var key in keys)
            {
                if (!items.Contains(key))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the named property value of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property value to get.</typeparam>
        /// <param name="object">The <see cref="PSObject"/> that may contain the named property.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <param name="default">
        /// Optional default value to return if the property is not defined.
        /// The default parameter value is the default value of type <typeparamref name="T"/>.
        /// </param>
        /// <returns>The value of the named property if defined; otherwise, the <paramref name="default"/> value.</returns>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is an empty string.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="object"/> or <paramref name="propertyName"/> is null.</exception>
        public static T GetPropertyValue<T>(this PSObject @object, string propertyName, T @default = default(T))
        {
            Validate.NotNull(@object, nameof(@object));
            Validate.NotNullOrEmpty(propertyName, nameof(propertyName));

            var property = @object.Properties.Match(propertyName, PSMemberTypes.Properties).FirstOrDefault();
            if (property != null)
            {
                if (property.Value is T)
                {
                    return (T)property.Value;
                }
                else
                {
                    return LanguagePrimitives.ConvertTo<T>(property.Value);
                }
            }

            return @default;
        }
    }
}
