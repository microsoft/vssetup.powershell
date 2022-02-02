// <copyright file="PackageReferenceTests.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Setup.Configuration;
    using Moq;
    using Xunit;

    public class PackageReferenceTests
    {
        [Fact]
        public void New_Reference_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>("reference", () => new PackageReference(null));
        }

        [Fact]
        public void New_Missing_Id_Throws()
        {
            var reference = new Mock<ISetupPackageReference>();
            reference.Setup(x => x.GetId()).Throws(new COMException("Not found", NativeMethods.E_NOTFOUND));

            var ex = Assert.Throws<COMException>(() => new PackageReference(reference.Object));
            Assert.Equal(unchecked((int)0x80070490), ex.ErrorCode);
        }

        [Fact]
        public void New_Missing_Version()
        {
            var reference = new Mock<ISetupPackageReference>();
            reference.Setup(x => x.GetId()).Returns("test");
            reference.Setup(x => x.GetVersion()).Throws(new COMException("Not found", NativeMethods.E_NOTFOUND));

            var sut = new PackageReference(reference.Object);
            Assert.Null(sut.Version);
        }

        [Fact]
        public void New_Null_Version()
        {
            var reference = new Mock<ISetupPackageReference>();
            reference.Setup(x => x.GetId()).Returns("test");
            reference.Setup(x => x.GetVersion()).Returns<string>(null);

            var sut = new PackageReference(reference.Object);
            Assert.Null(sut.Version);
        }

        [Fact]
        public void New_Invalid_Version()
        {
            var reference = new Mock<ISetupPackageReference>();
            reference.Setup(x => x.GetId()).Returns("test");
            reference.Setup(x => x.GetVersion()).Returns("invalid");

            var sut = new PackageReference(reference.Object);
            Assert.Null(sut.Version);
        }

        [Fact]
        public void New_Version()
        {
            var reference = new Mock<ISetupPackageReference>();
            reference.Setup(x => x.GetId()).Returns("test");
            reference.Setup(x => x.GetVersion()).Returns("1.2.3.4");

            var sut = new PackageReference(reference.Object);
            Assert.Equal(new Version(1, 2, 3, 4), sut.Version);
        }

        [Fact]
        public void ToString_Null_UniqueID_Is_Type()
        {
            var reference = new Mock<ISetupPackageReference>();
            reference.Setup(x => x.GetId()).Returns("test");
            reference.Setup(x => x.GetUniqueId()).Returns<string>(null);

            var sut = new PackageReference(reference.Object);
            Assert.Equal(typeof(PackageReference).FullName, sut.ToString());
        }

        [Fact]
        public void ToString_Is_UniqueId()
        {
            var reference = new Mock<ISetupPackageReference>();
            reference.Setup(x => x.GetId()).Returns("test");
            reference.Setup(x => x.GetUniqueId()).Returns("test,version=1.2.3.4");

            var sut = new PackageReference(reference.Object);
            Assert.Equal("test,version=1.2.3.4", sut.ToString());
        }
    }
}
