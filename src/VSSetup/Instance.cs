// <copyright file="Instance.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Configuration;

    /// <summary>
    /// Represents an instance of an installed product.
    /// </summary>
    public class Instance
    {
        private readonly string installationName;
        private readonly string installationPath;
        private readonly Version installationVersion;
        private readonly DateTime installDate;
        private readonly InstanceState state;
        private readonly string displayName;
        private readonly string description;
        private readonly string productPath;
        private readonly PackageReference product;
        private readonly IList<PackageReference> packages;

        /// <summary>
        /// Initializes a new instance of the <see cref="Instance"/> class.
        /// </summary>
        /// <param name="instance">The <see cref="ISetupInstance2"/> to adapt.</param>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is null.</exception>
        internal Instance(ISetupInstance2 instance)
        {
            Validate.NotNull(instance, nameof(instance));

            // The instance ID is required, but then try to set other properties to release the COM object and its resources ASAP.
            InstanceId = instance.GetInstanceId();

            TrySet(ref installationName, nameof(InstallationName), instance.GetInstallationName);
            TrySet(ref installationPath, nameof(InstallationPath), instance.GetInstallationPath);
            TrySet(ref installationVersion, nameof(InstallationVersion), () =>
            {
                Version version;

                var versionString = instance.GetInstallationVersion();
                if (Version.TryParse(versionString, out version))
                {
                    return version;
                }

                return null;
            });

            TrySet(ref installDate, nameof(InstallDate), () =>
            {
                var ft = instance.GetInstallDate();
                var l = ((long)ft.dwHighDateTime << 32) + ft.dwLowDateTime;

                return DateTime.FromFileTime(l);
            });

            TrySet(ref state, nameof(State), instance.GetState);

            var lcid = CultureInfo.CurrentUICulture.LCID;
            TrySet(ref displayName, nameof(DisplayName), () =>
            {
                return instance.GetDisplayName(lcid);
            });

            TrySet(ref description, nameof(Description), () =>
            {
                return instance.GetDescription(lcid);
            });

            TrySet(ref productPath, nameof(ProductPath), () =>
            {
                var path = instance.GetProductPath();
                return instance.ResolvePath(path);
            });

            TrySet(ref product, nameof(Product), () =>
            {
                var reference = instance.GetProduct();
                if (reference != null)
                {
                    return new PackageReference(reference);
                }

                return null;
            });

            TrySet(ref packages, nameof(Packages), () =>
            {
                return new List<PackageReference>(GetPackages(instance));
            });

            if (packages != null && packages.Any())
            {
                Packages = new ReadOnlyCollection<PackageReference>(packages);
            }
        }

        /// <summary>
        /// Gets the instance ID.
        /// </summary>
        public string InstanceId { get; }

        /// <summary>
        /// Gets the name of the installation.
        /// </summary>
        public string InstallationName => installationName;

        /// <summary>
        /// Gets the root path to the installation.
        /// </summary>
        public string InstallationPath => installationPath;

        /// <summary>
        /// Gets the version of the installed product.
        /// </summary>
        public Version InstallationVersion => installationVersion;

        /// <summary>
        /// Gets the <see cref="DateTime"/> when the product was installed.
        /// </summary>
        public DateTime InstallDate => installDate;

        /// <summary>
        /// Gets the <see cref="InstanceState"/> of the instance.
        /// </summary>
        public InstanceState State => state;

        /// <summary>
        /// Gets the display name using the current culture or fallback.
        /// </summary>
        public string DisplayName => displayName;

        /// <summary>
        /// Gets the description using the current culture or fallback.
        /// </summary>
        public string Description => description;

        /// <summary>
        /// Gets the path to the main product to launch.
        /// </summary>
        public string ProductPath => productPath;

        /// <summary>
        /// Gets a <see cref="PackageReference"/> for the installed product.
        /// </summary>
        public PackageReference Product => product;

        /// <summary>
        /// Gets a collection of <see cref="PackageReference"/> installed to the instance.
        /// </summary>
        public ReadOnlyCollection<PackageReference> Packages { get; }

        private static IEnumerable<PackageReference> GetPackages(ISetupInstance2 instance)
        {
            var references = instance.GetPackages();
            if (references != null)
            {
                foreach (var reference in instance.GetPackages())
                {
                    if (reference != null)
                    {
                        yield return new PackageReference(reference);
                    }
                }
            }
        }

        private void TrySet<T>(ref T property, string propertyName, Func<T> action)
        {
            try
            {
                property = action.Invoke();
            }
            catch (COMException ex) when (ex.ErrorCode == NativeMethods.E_NOTFOUND)
            {
                Trace.WriteLine($@"Instance: property ""{propertyName}"" not found on instance ""{InstanceId}"".");
            }
        }
    }
}
