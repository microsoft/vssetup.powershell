// <copyright file="GetInstanceCommand.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup.PowerShell
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using System.Runtime.InteropServices;
    using Configuration;
    using Properties;

    /// <summary>
    /// The Get-VSSetupInstance command.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "VSSetupInstance", DefaultParameterSetName = AllParameterSet)]
    [OutputType(typeof(Instance))]
    public class GetInstanceCommand : PSCmdlet
    {
        private const string AllParameterSet = "All";
        private const string PathParameterSet = "Path";
        private const string LiteralPathParameterSet = "LiteralPath";

        private ISetupConfiguration2 query = null;

        /// <summary>
        /// Gets or sets the All parameter.
        /// </summary>
        [Parameter(ParameterSetName = AllParameterSet)]
        public SwitchParameter All { get; set; }

        /// <summary>
        /// Gets or sets the Path parameter.
        /// </summary>
        [Parameter(ParameterSetName = PathParameterSet, Position = 0, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public string[] Path { get; set; }

        /// <summary>
        /// Gets or sets the LiteralPath (PSPath) parameter.
        /// </summary>
        [Alias("PSPath")]
        [Parameter(ParameterSetName = LiteralPathParameterSet, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string[] LiteralPath
        {
            get { return Path; }
            set { Path = value; }
        }

        private bool IsLiteralPath
        {
            get { return LiteralPathParameterSet.Equals(ParameterSetName, StringComparison.OrdinalIgnoreCase); }
        }

        /// <inheritdoc/>
        protected override void BeginProcessing()
        {
            query = new SetupConfiguration();
        }

        /// <inheritdoc/>
        protected override void ProcessRecord()
        {
            if (AllParameterSet.Equals(ParameterSetName, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var instance in GetInstances())
                {
                    WriteInstance(instance);
                }
            }
            else
            {
                var items = InvokeProvider.Item.Get(Path, true, IsLiteralPath) ?? Enumerable.Empty<PSObject>();
                foreach (var item in items)
                {
                    var path = item.GetPropertyValue<string>("PSPath");
                    var providerPath = SessionState.Path.GetUnresolvedProviderPathFromPSPath(path);

                    try
                    {
                        var instance = (ISetupInstance2)query.GetInstanceForPath(providerPath);
                        WriteInstance(instance);
                    }
                    catch (COMException ex) when (ex.ErrorCode == NativeMethods.E_NOTFOUND)
                    {
                        var message = string.Format(Resources.NoInstanceForPath_Args1, providerPath);
                        WriteWarning(message);
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override void EndProcessing()
        {
            // Release the COM object and its resources ASAP.
            query = null;
        }

        private IEnumerable<ISetupInstance2> GetInstances()
        {
            var e = All ? query.EnumAllInstances() : query.EnumInstances();
            int fetched;

            do
            {
                fetched = 0;
                ISetupInstance[] instances = new ISetupInstance[1];

                e.Next(1, instances, out fetched);
                if (fetched != 0)
                {
                    yield return (ISetupInstance2)instances[0];
                }
            }
            while (fetched != 0);
        }

        private void WriteInstance(ISetupInstance2 instance)
        {
            if (instance != null)
            {
                var adapted = new Instance(instance);
                WriteObject(adapted);
            }
        }
    }
}
