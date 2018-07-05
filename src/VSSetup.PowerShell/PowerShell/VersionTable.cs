// <copyright file="VersionTable.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup.PowerShell
{
    using System;
    using System.Collections;
    using System.Management.Automation;
    using System.Reflection;

    /// <summary>
    /// Dynamic property containing version information about this module.
    /// </summary>
    public sealed class VersionTable : PSVariable
    {
        private readonly Lazy<Hashtable> properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionTable"/> class.
        /// </summary>
        public VersionTable()
            : base("VSSetupVersionTable", null, ScopedItemOptions.None)
        {
            properties = new Lazy<Hashtable>(() =>
            {
                var thisVersionAttr = GetType().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
                Utilities.TryParseVersion(thisVersionAttr?.Version, out var thisVersion);

                return new Hashtable
                {
                    ["ModuleVersion"] = thisVersion,
                    ["QueryVersion"] = TryGetQueryVersion(),
                };
            });
        }

        /// <inheritdoc/>
        public override object Value => properties.Value;

        private static Version TryGetQueryVersion()
        {
            try
            {
                var query = QueryFactory.Create();
                if (Setup.Module.TryFromComObject(query, out var module))
                {
                    return module.Version;
                }
            }
            catch
            {
            }

            return null;
        }
    }
}
