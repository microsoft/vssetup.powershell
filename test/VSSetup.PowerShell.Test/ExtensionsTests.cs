// <copyright file="ExtensionsTests.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using Xunit;

    public class ExtensionsTests
    {
        public void Normalize_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>("version", () => Extensions.Normalize(null));
        }

        [Theory]
        [InlineData("1.2", "1.2.0.0")]
        [InlineData("1.2.3", "1.2.3.0")]
        [InlineData("1.2.3.4", "1.2.3.4")]
        public void Normalize(string value, string expected)
        {
            var actual = Version.Parse(value).Normalize();
            Assert.Equal(expected, actual.ToString());
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("a", "A")]
        [InlineData("A", "A")]
        [InlineData("ab", "Ab")]
        [InlineData("Ab", "Ab")]
        [InlineData("AB", "AB")]
        public void ToPascalCase(string value, string expected)
        {
            var actual = value.ToPascalCase();
            Assert.Equal(expected, actual);
        }
    }
}
