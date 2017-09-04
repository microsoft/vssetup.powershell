// <copyright file="Lazy.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;

    /// <summary>
    /// Creates an instance of <typeparamref name="T"/> only when <see cref="Value"/> is first accessed.
    /// </summary>
    /// <typeparam name="T">The type to create when <see cref="Value"/> is first accessed.</typeparam>
    internal class Lazy<T> : IDisposable
    {
        private readonly Func<T> factory;
        private bool hasValue = false;
        private T value = default(T);

        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy{T}"/> class.
        /// </summary>
        /// <param name="factory">A <see cref="Func{TResult}"/> to create an instance of the type.</param>
        public Lazy(Func<T> factory)
        {
            Validate.NotNull(factory, nameof(factory));
            this.factory = factory;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Value"/> has been created.
        /// </summary>
        public bool HasValue
        {
            get
            {
                lock (this)
                {
                    return hasValue;
                }
            }
        }

        /// <summary>
        /// Gets an instance of the type.
        /// </summary>
        public T Value
        {
            get
            {
                if (!hasValue)
                {
                    lock (this)
                    {
                        if (!hasValue)
                        {
                            value = factory();
                            hasValue = true;
                        }
                    }
                }

                return value;
            }
        }

        /// <inheritdoc/>
        void IDisposable.Dispose()
        {
            if (value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
