// <copyright file="Extensions.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Extension methods.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Gets a custom attribute of type <typeparamref name="T"/> defined on the assembly.
        /// </summary>
        /// <typeparam name="T">The type of attribute to get.</typeparam>
        /// <param name="source">The <see cref="Assembly"/> on which the custom attribute may be defined.</param>
        /// <returns>A custom attribute of type <typeparamref name="T"/> defined on the assembly.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static T GetCustomAttribute<T>(this Assembly source)
            where T : Attribute
        {
            Validate.NotNull(source, nameof(source));

            return source.GetCustomAttributes<T>()?.FirstOrDefault();
        }

        /// <summary>
        /// Gets a collection of custom attributes of type <typeparamref name="T"/> defined on the assembly.
        /// </summary>
        /// <typeparam name="T">The type of attributes to get.</typeparam>
        /// <param name="source">The <see cref="Assembly"/> on which the custom attributes may be defined.</param>
        /// <returns>A collection of custom attributes of type <typeparamref name="T"/> defined on the assembly.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IEnumerable<T> GetCustomAttributes<T>(this Assembly source)
            where T : Attribute
        {
            Validate.NotNull(source, nameof(source));

            return source.GetCustomAttributes(typeof(T), false)?.OfType<T>();
        }

        /// <summary>
        /// Returns a <see cref="Version"/> without negative fields.
        /// </summary>
        /// <param name="version">The <see cref="Version"/> to normalize.</param>
        /// <returns>A <see cref="Version"/> without negative fields.</returns>
        public static Version Normalize(this Version version)
        {
            Validate.NotNull(version, nameof(version));

            var build = version.Build > 0 ? version.Build : 0;
            var revision = version.Revision > 0 ? version.Revision : 0;

            return new Version(version.Major, version.Minor, build, revision);
        }

        /// <summary>
        /// Converts a string to PascalCase.
        /// </summary>
        /// <param name="value">The string value to convert.</param>
        /// <returns>If the string is not null or empty, a string converted to PascalCase; otherwise, the original string value.</returns>
        public static string ToPascalCase(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var sb = new StringBuilder(value);
                sb[0] = char.ToUpperInvariant(sb[0]);

                return sb.ToString();
            }

            return value;
        }
    }
}
