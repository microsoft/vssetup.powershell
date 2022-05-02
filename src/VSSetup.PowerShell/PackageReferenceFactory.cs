// <copyright file="PackageReferenceFactory.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using Microsoft.VisualStudio.Setup.Configuration;

    /// <summary>
    /// Creates <see cref="PackageReference"/> or derivative classes.
    /// </summary>
    internal static class PackageReferenceFactory
    {
        /// <summary>
        /// Creates a new <see cref="PackageReference"/> from an <see cref="ISetupPackageReference"/>.
        /// </summary>
        /// <param name="package">The <see cref="ISetupPackageReference"/> to wrap.</param>
        /// <returns>A <see cref="PackageReference"/> that wraps the <see cref="ISetupPackageReference"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="package"/> is null.</exception>
        public static PackageReference Create(ISetupPackageReference package)
        {
            Validate.NotNull(package, nameof(package));

            return new PackageReference(package);
        }

        /// <summary>
        /// Creates a new <see cref="FailedPackageReference"/> from an <see cref="ISetupFailedPackageReference"/>.
        /// </summary>
        /// <param name="package">The <see cref="ISetupFailedPackageReference"/> to wrap.</param>
        /// <returns>A <see cref="FailedPackageReference"/> that wraps the <see cref="ISetupFailedPackageReference"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="package"/> is null.</exception>
        public static FailedPackageReference Create(ISetupFailedPackageReference package)
        {
            Validate.NotNull(package, nameof(package));

            return new FailedPackageReference(package);
        }
    }
}
