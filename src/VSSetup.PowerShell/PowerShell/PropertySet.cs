// <copyright file="PropertySet.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup.PowerShell
{
    using System;
    using System.Collections.ObjectModel;
    using System.Management.Automation;

    /// <summary>
    /// A collection of <see cref="PSAdaptedProperty"/> objects indexed by their <see cref="PSMemberInfo.Name"/>.
    /// </summary>
    internal class PropertySet : KeyedCollection<string, PSAdaptedProperty>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertySet"/> class.
        /// </summary>
        public PropertySet()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// Gets the named <see cref="PSAdaptedProperty"/> or null if not found.
        /// </summary>
        /// <param name="propertyName">the name of the property to find.</param>
        /// <returns>The named <see cref="PSAdaptedProperty"/> or null if not found.</returns>
        public PSAdaptedProperty TryGet(string propertyName)
        {
            if (Contains(propertyName))
            {
                return this[propertyName];
            }

            return null;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is null.</exception>
        protected override string GetKeyForItem(PSAdaptedProperty item)
        {
            Validate.NotNull(item, nameof(item));

            return item.Name;
        }
    }
}
