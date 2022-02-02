// <copyright file="FailedPackageReference.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using Microsoft.VisualStudio.Setup.Configuration;

    /// <summary>
    /// Represents a failed package reference.
    /// </summary>
    public class FailedPackageReference : PackageReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedPackageReference"/> class.
        /// </summary>
        /// <param name="reference">The <see cref="ISetupFailedPackageReference"/> to adapt.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reference"/> is null.</exception>
        internal FailedPackageReference(ISetupFailedPackageReference reference)
            : base(reference)
        {
            Validate.NotNull(reference, nameof(reference));
        }
    }
}
