// <copyright file="ValidateTests.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using Xunit;

    public class ValidateTests
    {
        [Fact]
        public void NotNull_Throws()
        {
            Assert.Throws<ArgumentNullException>("test", () => Validate.NotNull<object>(null, "test"));
        }

        [Fact]
        public void NotNull()
        {
            Validate.NotNull("test", "test");
        }

        [Fact]
        public void NotEmpty_Throws()
        {
            Assert.Throws<ArgumentException>("test", () => Validate.NotEmpty(string.Empty, "test"));
        }

        [Fact]
        public void NotEmpty()
        {
            Validate.NotEmpty("test", "test");
        }

        [Fact]
        public void NotNullOrEmpty_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>("test", () => Validate.NotNullOrEmpty(null, "test"));
        }

        [Fact]
        public void NotNullOrEmpty_Empty_Throws()
        {
            Assert.Throws<ArgumentException>("test", () => Validate.NotNullOrEmpty(string.Empty, "test"));
        }

        [Fact]
        public void NotNullOrEmpty()
        {
            Validate.NotNullOrEmpty("test", "test");
        }
    }
}
