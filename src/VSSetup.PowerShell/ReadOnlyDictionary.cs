// <copyright file="ReadOnlyDictionary.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Wraps an <see cref="IDictionary{TKey, TValue}"/> in a read-only container.
    /// </summary>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <typeparam name="TValue">The type of value.</typeparam>
    internal class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        /// <summary>
        /// Gets an empty read-only dictionary.
        /// </summary>
        public static readonly ReadOnlyDictionary<TKey, TValue> Empty = new ReadOnlyDictionary<TKey, TValue>(
            new Dictionary<TKey, TValue>());

        // TODO: Use Framework-defined class if ever we upgrade minimum dependency.
        private readonly IDictionary<TKey, TValue> dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary to wrap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is null.</exception>
        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            Validate.NotNull(dictionary, nameof(dictionary));

            this.dictionary = dictionary;
        }

        /// <summary>
        /// Gets the number of elements in the read-only dictionary.
        /// </summary>
        public int Count => dictionary.Count;

        /// <summary>
        /// Gets an enumerable collection that contains the keys in the read-only dictionary.
        /// </summary>
        public ICollection<TKey> Keys => dictionary.Keys;

        /// <summary>
        /// Gets an enumerable collection that contains the values in the read-only dictionary.
        /// </summary>
        public ICollection<TValue> Values => dictionary.Values;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => true;

        /// <summary>
        /// Gets the element that has the specified key in the read-only dictionary.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>The element that has the specified key in the read-only dictionary.</returns>
        /// <exception cref="NotSupportedException">Setting a value is not supported.</exception>
        public TValue this[TKey key]
        {
            get { return dictionary[key]; }
            set { throw new NotSupportedException(); }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => dictionary.Contains(item);

        /// <summary>
        /// Determines whether the dictionary contains an element that has the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <returns>true if the dictionary contains an element that has the specified key; otherwise, false.</returns>
        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => dictionary.CopyTo(array, arrayIndex);

        /// <summary>
        /// Returns an enumerator that iterates through the read-only dictionary.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the read-only dictionary.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dictionary.GetEnumerator();

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Retrieves the value that is associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value will be retrieved.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the read-only dictionary contains an element with the specified key; otherwise, false.</returns>
        public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
