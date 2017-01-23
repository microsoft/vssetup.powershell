// <copyright file="NativeMethods.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    /// <summary>
    /// Native methods and constants.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Element not found (as HRESULT).
        /// </summary>
        public const int E_NOTFOUND = unchecked((int)0x80070490);

        /// <summary>
        /// Class not registered.
        /// </summary>
        public const int REGDB_E_CLASSNOTREG = unchecked((int)0x80040154);
    }
}
