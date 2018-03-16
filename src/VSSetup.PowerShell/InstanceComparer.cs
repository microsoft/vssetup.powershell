// <copyright file="InstanceComparer.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Compares instances of <see cref="Instance"/>.
    /// </summary>
    internal class InstanceComparer : IComparer<Instance>
    {
        /// <summary>
        /// Gets an instance of the <see cref="InstanceComparer"/> that compares only the
        /// <see cref="Instance.InstallationVersion"/> and <see cref="Instance.InstallDate"/> properties.
        /// </summary>
        public static readonly IComparer<Instance> VersionAndDate = new InstanceComparer();

        /// <inheritdoc/>
        public int Compare(Instance x, Instance y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }
            else if (x is null)
            {
                return -1;
            }
            else if (y is null)
            {
                return 1;
            }

            var result = Compare(x.InstallationVersion, y.InstallationVersion);
            if (result != 0)
            {
                return result;
            }

            return DateTime.Compare(x.InstallDate, y.InstallDate);
        }

        private static int Compare(Version x, Version y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }
            else if (x is null)
            {
                return -1;
            }
            else if (y is null)
            {
                return 1;
            }

            return x.CompareTo(y);
        }
    }
}
