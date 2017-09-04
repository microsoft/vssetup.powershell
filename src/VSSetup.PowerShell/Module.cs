// <copyright file="Module.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// A native module.
    /// </summary>
    internal class Module : IDisposable
    {
        private readonly SafeModuleHandle handle;
        private string path = null;

        private Module(SafeModuleHandle handle)
        {
            this.handle = handle;
        }

        ~Module()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets a value indicating whether the object is already disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets the path to the <see cref="Module"/>.
        /// </summary>
        public string Path
        {
            get
            {
                if (path == null)
                {
                    var sb = new StringBuilder(NativeMethods.MAX_PATH);
                    var length = NativeMethods.GetModuleFileName(handle, sb, sb.Capacity);

                    if (length > 0)
                    {
                        path = sb.ToString();
                    }
                }

                return path;
            }
        }

        /// <summary>
        /// Gets the <see cref="Version"/> of the module.
        /// </summary>
        public Version Version
        {
            get
            {
                var path = Path;

                if (!string.IsNullOrEmpty(path))
                {
                    var info = FileVersionInfo.GetVersionInfo(path);
                    return new Version(info.FileMajorPart, info.FileMinorPart, info.FileBuildPart, info.FilePrivatePart);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a <see cref="Module"/> for a runtime-callable wrapper.
        /// </summary>
        /// <param name="object">An <see cref="object"/> that represents the COM object.</param>
        /// <param name="module">The <see cref="Module"/> if the <paramref name="object"/> is a COM object.</param>
        /// <returns>True if we could get the <see cref="Module"/> for a runtime-callable wrapper for the given COM object; otherwise, false.</returns>
        public static bool TryFromComObject(object @object, out Module module)
        {
            if (@object != null)
            {
                var unk = Marshal.GetIUnknownForObject(@object);
                if (unk != IntPtr.Zero)
                {
                    try
                    {
                        var addr = Marshal.ReadIntPtr(unk);
                        if (NativeMethods.GetModuleHandleEx(GetModuleHandleExFlags.FromAddress, addr, out var handle))
                        {
                            module = new Module(handle);
                            return true;
                        }
                    }
                    finally
                    {
                        Marshal.Release(unk);
                    }
                }
            }

            module = null;
            return false;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if the object is being disposed; otherwise, false if the object is being finalized.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (!handle.IsInvalid)
                {
                    handle.Dispose();
                }

                IsDisposed = true;
            }
        }
    }
}
