// <copyright file="Extensions.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup.PowerShell
{
    using System;
    using System.Linq;
    using System.Management.Automation;

    /// <summary>
    /// Extension methods.
    /// </summary>
    internal static class Extensions
    {
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
