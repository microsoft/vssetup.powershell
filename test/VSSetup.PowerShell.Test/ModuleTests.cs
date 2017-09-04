// <copyright file="ModuleTests.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using Microsoft.VisualStudio.Setup.Configuration;
    using Xunit;

    public class ModuleTests
    {
        [Fact]
        public void TryFromComObject_Null()
        {
            Assert.False(Module.TryFromComObject(null, out var module));
            Assert.Null(module);
        }

        [Fact]
        public void TryFromComObject_Not_Com()
        {
            Assert.False(Module.TryFromComObject(string.Empty, out var module));
            Assert.Null(module);
        }

        [Fact]
        public void TryFromComObject()
        {
            var query = new SetupConfiguration();

            var result = Module.TryFromComObject(query, out var module);
            using (module)
            {
                Assert.True(result);
                Assert.NotNull(module);
            }
        }
    }
}
