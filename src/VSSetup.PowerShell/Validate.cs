// <copyright file="Validate.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using Microsoft.VisualStudio.Setup.Properties;

    /// <summary>
    /// Argument validation utility methods.
    /// </summary>
    internal static class Validate
    {
        /// <summary>
        /// Validates that the reference <paramref name="object"/> is not null.
        /// </summary>
        /// <typeparam name="T">The type of reference object to validate.</typeparam>
        /// <param name="object">The reference object to validate.</param>
        /// <param name="paramName">The name of the parameter to validate.</param>
        /// <exception cref="ArgumentNullException"><paramref name="object"/> is null.</exception>
        public static void NotNull<T>(T @object, string paramName)
            where T : class
        {
            if (@object == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// Validates that the string <paramref name="value"/> is not an empty string.
        /// </summary>
        /// <param name="value">The string to validate.</param>
        /// <param name="paramName">The name of the parameter to validate.</param>
        /// <exception cref="ArgumentException"><paramref name="value"/> is an empty string.</exception>
        public static void NotEmpty(string value, string paramName)
        {
            if (value == string.Empty)
            {
                throw new ArgumentException(Resources.EmptyString, paramName);
            }
        }

        /// <summary>
        /// Validates that the string <paramref name="value"/> is not null or an empty string.
        /// </summary>
        /// <param name="value">The string to validate.</param>
        /// <param name="paramName">The name of the parameter to validate.</param>
        /// <exception cref="ArgumentException"><paramref name="value"/> is an empty string.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        public static void NotNullOrEmpty(string value, string paramName)
        {
            NotNull(value, paramName);
            NotEmpty(value, paramName);
        }
    }
}
