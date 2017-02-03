// <copyright file="PackageReference.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using Configuration;

    /// <summary>
    /// Represents a package reference.
    /// </summary>
    public class PackageReference
    {
        private readonly Version version;
        private readonly string chip;
        private readonly string branch;
        private readonly string type;
        private readonly bool isExtension;
        private readonly string uniqueId;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageReference"/> class.
        /// </summary>
        /// <param name="reference">The <see cref="ISetupPackageReference"/> to adapt.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reference"/> is null.</exception>
        internal PackageReference(ISetupPackageReference reference)
        {
            Validate.NotNull(reference, nameof(reference));

            // The package reference ID is required, but then try to set other properties to release the COM object and its resources ASAP.
            Id = reference.GetId();

            Utilities.TrySet(ref version, nameof(Version), () =>
            {
                Version version;

                var versionString = reference.GetVersion();
                if (Version.TryParse(versionString, out version))
                {
                    return version;
                }

                return null;
            });

            Utilities.TrySet(ref chip, nameof(Chip), reference.GetChip);
            Utilities.TrySet(ref branch, nameof(Branch), reference.GetBranch);
            Utilities.TrySet(ref type, nameof(Type), reference.GetType);
            Utilities.TrySet(ref isExtension, nameof(IsExtension), reference.GetIsExtension);
            Utilities.TrySet(ref uniqueId, nameof(UniqueId), reference.GetUniqueId);
        }

        /// <summary>
        /// Gets the ID of the referenced package.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the <see cref="Version"/> of the referenced package.
        /// </summary>
        public Version Version => version;

        /// <summary>
        /// Gets the chip of the referenced package.
        /// </summary>
        public string Chip => chip;

        /// <summary>
        /// Gets the branch of the referenced package.
        /// </summary>
        public string Branch => branch;

        /// <summary>
        /// Gets the type of the referenced package.
        /// </summary>
        public string Type => type;

        /// <summary>
        /// Gets a value indicating whether the package is an extension.
        /// </summary>
        public bool IsExtension => isExtension;

        /// <summary>
        /// Gets the unique package ID.
        /// </summary>
        public string UniqueId => uniqueId;

        /// <summary>
        /// Returns the unique package ID of the package.
        /// </summary>
        /// <returns>The unique package ID of the package.</returns>
        public override string ToString()
        {
            return uniqueId ?? base.ToString();
        }
    }
}
