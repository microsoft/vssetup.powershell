// <copyright file="Errors.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Configuration;

    /// <summary>
    /// Represents errors that occurred during the last install operation.
    /// </summary>
    public class Errors
    {
        private readonly IList<FailedPackageReference> failedPackages;
        private readonly IList<PackageReference> skippedPackages;

        internal Errors(ISetupErrorState errors)
        {
            Validate.NotNull(errors, nameof(errors));

            FailedPackages = Utilities.TrySetCollection(ref failedPackages, nameof(FailedPackages), errors.GetFailedPackages, PackageReferenceFactory.Create);
            SkippedPackages = Utilities.TrySetCollection(ref skippedPackages, nameof(SkippedPackages), errors.GetSkippedPackages, PackageReferenceFactory.Create);
        }

        /// <summary>
        /// Gets a collection of references to packages that failed to install.
        /// </summary>
        public ReadOnlyCollection<FailedPackageReference> FailedPackages { get; }

        /// <summary>
        /// Gets a collection of references to packages skipped because other packages in their parent workload or components failed.
        /// </summary>
        public ReadOnlyCollection<PackageReference> SkippedPackages { get; }
    }
}
