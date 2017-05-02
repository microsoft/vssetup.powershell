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
    using System.Reflection;
    using Configuration;

    /// <summary>
    /// Represents an instance of an installed product.
    /// </summary>
    public class Instance
    {
        private static readonly ISet<string> DeclaredProperties;

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
        private readonly IDictionary<string, object> properties;
        private readonly string enginePath;
        private readonly bool isComplete;
        private readonly bool isLaunchable;

        static Instance()
        {
            var properties = typeof(Instance)
                .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                .Select(property => property.Name);

            DeclaredProperties = new HashSet<string>(properties, StringComparer.OrdinalIgnoreCase);
        }

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

            Utilities.TrySet(ref installationName, nameof(InstallationName), instance.GetInstallationName, OnError);
            Utilities.TrySet(ref installationPath, nameof(InstallationPath), instance.GetInstallationPath, OnError);
            Utilities.TrySet(
                ref installationVersion,
                nameof(InstallationVersion),
                () =>
                {
                    var versionString = instance.GetInstallationVersion();
                    if (Version.TryParse(versionString, out Version version))
                    {
                        return version.Normalize();
                    }

                    return null;
                },
                OnError);

            Utilities.TrySet(
                ref installDate,
                nameof(InstallDate),
                () =>
                {
                    var ft = instance.GetInstallDate();
                    var l = ((long)ft.dwHighDateTime << 32) + ft.dwLowDateTime;

                    return DateTime.FromFileTime(l);
                },
                OnError);

            Utilities.TrySet(ref state, nameof(State), instance.GetState, OnError);

            var lcid = CultureInfo.CurrentUICulture.LCID;
            Utilities.TrySet(
                ref displayName,
                nameof(DisplayName),
                () =>
                {
                    return instance.GetDisplayName(lcid);
                },
                OnError);

            Utilities.TrySet(
                ref description,
                nameof(Description),
                () =>
                {
                    return instance.GetDescription(lcid);
                },
                OnError);

            Utilities.TrySet(
                ref productPath,
                nameof(ProductPath),
                () =>
                {
                    var path = instance.GetProductPath();
                    return instance.ResolvePath(path);
                },
                OnError);

            Utilities.TrySet(
                ref product,
                nameof(Product),
                () =>
                {
                    var reference = instance.GetProduct();
                    if (reference != null)
                    {
                        return new PackageReference(reference);
                    }

                    return null;
                },
                OnError);

            Packages = Utilities.TrySetCollection(ref packages, nameof(Packages), instance.GetPackages, PackageReferenceFactory.Create, OnError);

            var errors = instance.GetErrors();
            if (errors != null)
            {
                Errors = new Errors(errors);
            }

            Utilities.TrySet(
                ref properties,
                nameof(Properties),
                () =>
                {
                    var properties = instance.GetProperties();
                    return properties?.GetNames()
                        .ToDictionary(name => name.ToPascalCase(), name => properties.GetValue(name), StringComparer.OrdinalIgnoreCase);
                },
                OnError);

            if (properties != null)
            {
                Properties = new ReadOnlyDictionary<string, object>(properties);
            }
            else
            {
                // While accessing properties on a null object succeeds in PowerShell, accessing the indexer does not.
                Properties = ReadOnlyDictionary<string, object>.Empty;
            }

            Utilities.TrySet(ref enginePath, nameof(EnginePath), instance.GetEnginePath, OnError);
            Utilities.TrySet(ref isComplete, nameof(IsComplete), instance.IsComplete, OnError);
            Utilities.TrySet(ref isLaunchable, nameof(IsLaunchable), instance.IsLaunchable, OnError);

            // Get all properties of the instance not explicitly declared.
            var store = (ISetupPropertyStore)instance;
            AdditionalProperties = store.GetNames()
                .Where(name => !DeclaredProperties.Contains(name))
                .ToDictionary(name => name.ToPascalCase(), name => store.GetValue(name), StringComparer.OrdinalIgnoreCase);
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

        /// <summary>
        /// Gets custom properties defined for the instance.
        /// </summary>
        public IDictionary<string, object> Properties { get; }

        /// <summary>
        /// Gets errors that occurred during the last install (if any).
        /// </summary>
        public Errors Errors { get; }

        /// <summary>
        /// Gets the path to the engine that installed the instance.
        /// </summary>
        public string EnginePath => enginePath;

        /// <summary>
        /// Gets a value indicating whether the instance is complete.
        /// </summary>
        public bool IsComplete => isComplete;

        /// <summary>
        /// Gets a value indicating whether the instance is launchable (e.g. may have errors but other features work).
        /// </summary>
        public bool IsLaunchable => isLaunchable;

        /// <summary>
        /// Gets additional properties not explicitly defined on this class.
        /// </summary>
        internal IDictionary<string, object> AdditionalProperties { get; }

        private void OnError(string propertyName)
        {
            Trace.WriteLine($@"Instance: property ""{propertyName}"" not found on instance ""{InstanceId}"".");
        }
    }
}
