// <copyright file="SelectInstanceCommand.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup.PowerShell
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Text.RegularExpressions;
    using Configuration;

    /// <summary>
    /// The Get-VSSetupInstance command.
    /// </summary>
    [Cmdlet(VerbsCommon.Select, "VSSetupInstance")]
    [OutputType(typeof(Instance))]
    public class SelectInstanceCommand : Cmdlet
    {
        /// <summary>
        /// A regular expression that matches a version range.
        /// </summary>
        internal const string VersionRangePattern = @"^((\d+(\.\d+){1,3})|([\[\(]\s*(\d+(\.\d+){1,3})?\s*(,\s*(\d+(\.\d+){1,3})?)?\s*[\]\)]))$";

        /// <summary>
        /// Regular expression options for the version range.
        /// </summary>
        internal const RegexOptions VersionRangeOptions = RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.Singleline;

        private ISetupHelper helper = null;
        private Instance latestInstance = null;
        private ulong minVersion = 0;
        private ulong maxVersion = 0;

        /// <summary>
        /// Gets or sets the Instance parameter.
        /// </summary>
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true)]
        public Instance[] Instance { get; set; }

        /// <summary>
        /// Gets or sets the Product parameter.
        /// </summary>
        [Parameter]
        [ValidateNotNullOrEmpty]
        public string[] Product { get; set; } = new[]
        {
            "Microsoft.VisualStudio.Product.Enterprise",
            "Microsoft.VisualStudio.Product.Professional",
            "Microsoft.VisualStudio.Product.Community",
        };

        /// <summary>
        /// Gets or sets the Require parameter.
        /// </summary>
        [Parameter]
        [ValidateNotNullOrEmpty]
        public string[] Require { get; set; }

        /// <summary>
        /// Gets or sets the Version parameter.
        /// </summary>
        [Parameter]
        [ValidatePattern(VersionRangePattern, Options = VersionRangeOptions)]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the Latest parameter.
        /// </summary>
        [Parameter]
        public SwitchParameter Latest { get; set; }

        /// <inheritdoc/>
        protected override void BeginProcessing()
        {
            if (!string.IsNullOrEmpty(Version))
            {
                var query = QueryFactory.Create();
                if (query != null)
                {
                    helper = (ISetupHelper)new SetupConfiguration();

                    helper.ParseVersionRange(Version, out minVersion, out maxVersion);
                }
            }
        }

        /// <inheritdoc/>
        protected override void ProcessRecord()
        {
            IEnumerable<Instance> instances = Instance;

            if (Product != null && Product.Any())
            {
                // Select instances with no product or the specified product ID.
                instances = from instance in instances
                            let instanceProduct = instance?.Product
                            where instanceProduct == null || Product.Contains(instanceProduct.Id, StringComparer.OrdinalIgnoreCase)
                            select instance;
            }

            if (Require != null && Require.Any())
            {
                // Select instances that contain all specified package IDs.
                instances = from instance in instances
                            let instancePackages = instance?.Packages
                            where instancePackages != null && instancePackages.ContainsAll(package => package.Id, Require, StringComparer.OrdinalIgnoreCase)
                            select instance;
            }

            if (helper != null)
            {
                instances = from instance in instances
                            let instanceVersion = instance?.InstallationVersion?.ToString()
                            where !string.IsNullOrEmpty(instanceVersion)
                            let version = helper.ParseVersion(instanceVersion)
                            where minVersion <= version && version <= maxVersion
                            select instance;
            }

            foreach (var instance in instances)
            {
                if (Latest)
                {
                    if (latestInstance == null || latestInstance.InstallDate < instance.InstallDate)
                    {
                        latestInstance = instance;
                    }
                }
                else
                {
                    WriteInstance(instance);
                }
            }
        }

        /// <inheritdoc/>
        protected override void EndProcessing()
        {
            if (latestInstance != null)
            {
                WriteInstance(latestInstance);
            }

            // Release the COM object and its resources ASAP.
            helper = null;
        }

        private void WriteInstance(Instance instance)
        {
            if (instance != null)
            {
                WriteObject(instance);
            }
        }
    }
}
