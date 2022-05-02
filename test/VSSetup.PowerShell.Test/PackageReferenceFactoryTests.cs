// <copyright file="PackageReferenceFactoryTests.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using Microsoft.VisualStudio.Setup.Configuration;
    using Moq;
    using Xunit;

    public class PackageReferenceFactoryTests
    {
        [Fact]
        public void Create_PackageReference_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>("package", () => PackageReferenceFactory.Create((ISetupPackageReference)null));
        }

        [Fact]
        public void Create_PackageReference()
        {
            var reference = Mock.Of<ISetupPackageReference>(x => x.GetId() == "a");
            var actual = PackageReferenceFactory.Create(reference);

            Assert.IsType<PackageReference>(actual);
        }

        [Fact]
        public void Create_FailedPackageReference_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>("package", () => PackageReferenceFactory.Create((ISetupFailedPackageReference)null));
        }

        [Fact]
        public void Create_FailedPackageReference()
        {
            var reference = new Mock<ISetupFailedPackageReference>();
            reference.As<ISetupPackageReference>().Setup(x => x.GetId()).Returns("a");

            var actual = PackageReferenceFactory.Create(reference.Object);

            Assert.IsType<FailedPackageReference>(actual);
        }
    }
}
