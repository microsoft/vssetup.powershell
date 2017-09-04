// <copyright file="NativeMethods.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

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
        /// The maximum path length (legacy).
        /// </summary>
        public const int MAX_PATH = 260;

        /// <summary>
        /// Class not registered.
        /// </summary>
        public const int REGDB_E_CLASSNOTREG = unchecked((int)0x80040154);

        /// <summary>
        /// Gets the file name of the module for the given handle.
        /// </summary>
        /// <param name="hModule">The handle of the module.</param>
        /// <param name="lpFilename">The file name of the module.</param>
        /// <param name="nSize">The size of the <paramref name="lpFilename"/> buffer.</param>
        /// <returns>The length of the string in <paramref name="lpFilename"/>; otherwise, 0 if the function fails and <see cref="Marshal.GetLastWin32Error"/> will contain the reason.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetModuleFileNameW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern int GetModuleFileName(
            SafeModuleHandle hModule,
            [Out] StringBuilder lpFilename,
            [MarshalAs(UnmanagedType.U4)] int nSize);

        /// <summary>
        /// Gets a handle to the module given the module name.
        /// </summary>
        /// <param name="dwFlags">The <see cref="GetModuleHandleExFlags"/> for the call.</param>
        /// <param name="lpModuleName">The name of the module or pointer to a function within the module.</param>
        /// <param name="phModule">A handle to the module.</param>
        /// <returns>True if the function succeeds; otherwise, false and <see cref="Marshal.GetLastWin32Error"/> will contain the reason.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetModuleHandleExW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetModuleHandleEx(
            [MarshalAs(UnmanagedType.U4)] GetModuleHandleExFlags dwFlags,
            IntPtr lpModuleName,
            out SafeModuleHandle phModule);

        /// <summary>
        /// Frees the module given its handle.
        /// </summary>
        /// <param name="hModule">The handle to the module to free.</param>
        /// <returns>True if the function succeeds; otherwise, false and <see cref="Marshal.GetLastWin32Error"/> will contain the reason.</returns>
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(
            IntPtr hModule);
    }

#pragma warning disable SA1201 // Elements must appear in the correct order

    /// <summary>
    /// Flags for <see cref="GetModuleHandleEx(GetModuleHandleExFlags, IntPtr, out SafeHandle)"/>.
    /// </summary>
    [Flags]
    public enum GetModuleHandleExFlags
    {
        /// <summary>
        /// The module name is a pointer to a function within the module.
        /// </summary>
        FromAddress = 4,
    }

#pragma warning restore SA1201 // Elements must appear in the correct order
}
