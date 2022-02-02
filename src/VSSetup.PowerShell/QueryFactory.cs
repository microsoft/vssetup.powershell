// <copyright file="QueryFactory.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System.IO;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Setup.Configuration;

    internal static class QueryFactory
    {
        /// <summary>
        /// Creates an instance of a class implementing <see cref="ISetupConfiguration2"/>.
        /// </summary>
        /// <returns>An instance of a class implementing <see cref="ISetupConfiguration2"/>.</returns>
        public static ISetupConfiguration2 Create()
        {
            try
            {
                return new SetupConfiguration();
            }
            catch (COMException ex) when (ex.ErrorCode == NativeMethods.REGDB_E_CLASSNOTREG)
            {
                return null;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }
    }
}
