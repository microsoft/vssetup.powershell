// <copyright file="SafeModuleHandle.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A handle to a module.
    /// </summary>
    internal sealed class SafeModuleHandle : SafeHandle
    {
        private static readonly IntPtr InvalidHandleValue = IntPtr.Zero;

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeModuleHandle"/> class.
        /// </summary>
        public SafeModuleHandle()
            : base(InvalidHandleValue, true)
        {
        }

        /// <inheritdoc/>
        public override bool IsInvalid => handle != InvalidHandleValue;

        /// <inheritdoc/>
        protected override bool ReleaseHandle()
        {
            return NativeMethods.FreeLibrary(handle);
        }
    }
}
