// <copyright file="FailedPackageReferenceTests.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using Microsoft.VisualStudio.Setup.Configuration;
    using Moq;
    using Xunit;

    public class FailedPackageReferenceTests
    {
        [Fact]
        public void New_Reference_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>("reference", () => new FailedPackageReference(null));
        }

        [Fact]
        public void New_Valid()
        {
            var reference = new Mock<ISetupFailedPackageReference>();
            reference.As<ISetupPackageReference>().Setup(x => x.GetId()).Returns("a");

            var sut = new FailedPackageReference(reference.Object);
            Assert.Equal("a", sut.Id);
        }
    }
}
