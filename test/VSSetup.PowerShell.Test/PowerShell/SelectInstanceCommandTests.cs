// <copyright file="SelectInstanceCommandTests.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup.PowerShell
{
    using System.Text.RegularExpressions;
    using Xunit;

    public class SelectInstanceCommandTests
    {
        [Theory]
        [InlineData("invalid", false)]
        [InlineData("1", "false")]
        [InlineData("1.0", true)]
        [InlineData("1.20.300", true)]
        [InlineData("1.20.300.4000", true)]
        [InlineData("1.2.3.4.5", false)]
        [InlineData("[1.0", false)]
        [InlineData("[1.0)", true)]
        [InlineData("[1.0,)", true)]
        [InlineData("[1.20.300.4000,)", true)]
        [InlineData("[1.20.300.4000,2.30.400.5000", false)]
        [InlineData("[1.20.300.4000,2.30.400.5000]", true)]
        [InlineData("[1.20.300.4000,2.30.400.5000)", true)]
        [InlineData("(1.20.300.4000,2.30.400.5000]", true)]
        [InlineData("(1.20.300.4000,2.30.400.5000)", true)]
        [InlineData("[ 1.20.300.4000 , 2.30.400.5000 )", true)]
        [InlineData("[  1. 20. 300. 4000  ,  2. 30. 400. 5000 )", false)]
        [InlineData("[,1.0)", true)]
        [InlineData(",1.0)", false)]
        public void Version_ValidatePattern(string value, bool expected)
        {
            var actual = Regex.IsMatch(value, SelectInstanceCommand.VersionRangePattern, SelectInstanceCommand.VersionRangeOptions);
            Assert.Equal(expected, actual);
        }
    }
}
