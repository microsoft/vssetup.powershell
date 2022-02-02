// <copyright file="Utilities.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>
#pragma warning disable SA1314 // Type parameter names should begin with T

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Setup.Configuration;

    /// <summary>
    /// Utility methods.
    /// </summary>
    internal static class Utilities
    {
        /// <summary>
        /// Gets an empty <see cref="ReadOnlyCollection{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of element.</typeparam>
        /// <returns>An empty <see cref="ReadOnlyCollection{T}"/>.</returns>
        public static ReadOnlyCollection<T> EmptyReadOnlyCollection<T>()
        {
            return EmptyReadOnlyCollectionContainer<T>.Instance;
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{T}"/> of adapted packages from type <typeparamref name="T"/> to type <typeparamref name="R"/>.
        /// </summary>
        /// <typeparam name="T">The type of the package reference to adapt.</typeparam>
        /// <typeparam name="R">The adapted type of the package reference.</typeparam>
        /// <param name="action">A method that gets an <see cref="IEnumerable{T}"/> of package reference of type <typeparamref name="T"/>.</param>
        /// <param name="creator">A method that creates an adapted reference of type <typeparamref name="R"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of adapted packages. This enumeration may yield zero results.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> or <paramref name="creator"/> is null.</exception>
        public static IEnumerable<R> GetAdaptedPackages<T, R>(Func<IEnumerable<T>> action, Func<T, R> creator)
            where T : ISetupPackageReference
            where R : PackageReference
        {
            Validate.NotNull(action, nameof(action));
            Validate.NotNull(creator, nameof(creator));

            return YieldAdaptedPackages(action, creator);
        }

        /// <summary>
        /// Tries to convert the string representation of a version number to an equivalent System.Version object,
        /// and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="input">A string that contains a version number to convert.</param>
        /// <param name="result">
        /// When this method returns, contains the System.Version equivalent of the number that is contained in input,
        /// if the conversion succeeded. If the conversion failed, or if input is null or System.String.Empty,
        /// result is null when the method returns.
        /// </param>
        /// <returns>true if the input parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParseVersion(string input, out Version result)
        {
            if (string.IsNullOrEmpty(input))
            {
                result = null;
                return false;
            }

            try
            {
                result = new Version(input);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Sets the given <paramref name="property"/> if the <paramref name="action"/> does not throw a <see cref="COMException"/> for 0x80070490.
        /// </summary>
        /// <typeparam name="T">The type of the property to set.</typeparam>
        /// <param name="property">A reference to the property to set.</param>
        /// <param name="propertyName">The name of the property for diagnostic purposes.</param>
        /// <param name="action">A method that returns the value of the property to set.</param>
        /// <param name="error">Optional error handler that accepts the name of the property.</param>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is an empty string.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> or <paramref name="action"/> is null.</exception>
        public static void TrySet<T>(ref T property, string propertyName, Func<T> action, Action<string> error = null)
        {
            Validate.NotNullOrEmpty(propertyName, nameof(propertyName));
            Validate.NotNull(action, nameof(action));

            try
            {
                property = action.Invoke();
            }
            catch (COMException ex) when (ex.ErrorCode == NativeMethods.E_NOTFOUND)
            {
                error?.Invoke(propertyName);
            }
        }

        /// <summary>
        /// Sets the given package reference collection <paramref name="property"/> if the <paramref name="action"/> does not throw a <see cref="COMException"/> for 0x80070490.
        /// </summary>
        /// <typeparam name="T">The type of the package reference to adapt.</typeparam>
        /// <typeparam name="R">The adapted type of the package reference.</typeparam>
        /// <param name="property">A reference to the property to set.</param>
        /// <param name="propertyName">The name of the property for diagnostic purposes.</param>
        /// <param name="action">A method that returns the value of the property to set.</param>
        /// <param name="creator">A method that creates the adapted reference type.</param>
        /// <param name="error">Optional error handler that accepts the name of the property.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing the adapted package references. This collection may be empty.</returns>
        /// <exception cref="ArgumentException"><paramref name="propertyName"/> is an empty string.</exception>
        /// <exception cref="ArgumentNullException">One or more parameters is null.</exception>
        public static ReadOnlyCollection<R> TrySetCollection<T, R>(
            ref IList<R> property,
            string propertyName,
            Func<IEnumerable<T>> action,
            Func<T, R> creator,
            Action<string> error = null)
            where T : ISetupPackageReference
            where R : PackageReference
        {
            Validate.NotNullOrEmpty(propertyName, nameof(propertyName));
            Validate.NotNull(action, nameof(action));
            Validate.NotNull(creator, nameof(creator));

            var packages = GetAdaptedPackages(action, creator);
            TrySet(ref property, propertyName, packages.ToList, error);

            if (property != null && property.Any())
            {
                return new ReadOnlyCollection<R>(property);
            }

            return EmptyReadOnlyCollection<R>();
        }

        private static IEnumerable<R> YieldAdaptedPackages<T, R>(Func<IEnumerable<T>> action, Func<T, R> creator)
        {
            var references = action?.Invoke();
            if (references != null)
            {
                foreach (var reference in references)
                {
                    if (reference != null)
                    {
                        yield return creator(reference);
                    }
                }
            }
        }

        private class EmptyReadOnlyCollectionContainer<T>
        {
            public static readonly ReadOnlyCollection<T> Instance = new ReadOnlyCollection<T>(new T[0]);
        }
    }
}
#pragma warning restore SA1314 // Type parameter names should begin with T
