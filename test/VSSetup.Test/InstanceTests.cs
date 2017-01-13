// <copyright file="InstanceTests.cs" company="Microsoft Corporation">
// Copyright (C) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root for license information.
// </copyright>

namespace Microsoft.VisualStudio.Setup
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Configuration;
    using Moq;
    using Xunit;

    public class InstanceTests
    {
        [Fact]
        public void New_Instance_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>("instance", () => new Instance(null));
        }

        [Fact]
        public void New_Missing_InstanceId_Throws()
        {
            var instance = new Mock<ISetupInstance2>();
            instance.Setup(x => x.GetInstanceId()).Throws(new COMException("Not found", NativeMethods.E_NOTFOUND));

            var ex = Assert.Throws<COMException>(() => new Instance(instance.Object));
            Assert.Equal(unchecked((int)0x80070490), ex.ErrorCode);
        }

        [Fact]
        public void New_Missing_InstallationName()
        {
            var instance = new Mock<ISetupInstance2>();
            instance.Setup(x => x.GetInstanceId()).Returns("test");
            instance.Setup(x => x.GetInstallationName()).Throws(new COMException("Not found", NativeMethods.E_NOTFOUND));

            var sut = new Instance(instance.Object);
            Assert.Null(sut.InstallationName);
        }

        [Fact]
        public void New_Null_InstallationVersion()
        {
            var instance = new Mock<ISetupInstance2>();
            instance.Setup(x => x.GetInstanceId()).Returns("test");
            instance.Setup(x => x.GetInstallationVersion()).Returns<string>(null);

            var sut = new Instance(instance.Object);
            Assert.Null(sut.InstallationVersion);
        }

        [Fact]
        public void New_Invalid_InstallationVersion()
        {
            var instance = new Mock<ISetupInstance2>();
            instance.Setup(x => x.GetInstanceId()).Returns("test");
            instance.Setup(x => x.GetInstallationVersion()).Returns("invalid");

            var sut = new Instance(instance.Object);
            Assert.Null(sut.InstallationVersion);
        }

        [Fact]
        public void New_InstallationVersion()
        {
            var instance = new Mock<ISetupInstance2>();
            instance.Setup(x => x.GetInstanceId()).Returns("test");
            instance.Setup(x => x.GetInstallationVersion()).Returns("1.2.3.4");

            var sut = new Instance(instance.Object);
            Assert.Equal(new Version(1, 2, 3, 4), sut.InstallationVersion);
        }

        [Fact]
        public void New_Product_Null()
        {
            var instance = new Mock<ISetupInstance2>();
            instance.Setup(x => x.GetInstanceId()).Returns("test");

            var sut = new Instance(instance.Object);
            Assert.Null(sut.Product);
        }

        [Fact]
        public void New_Product()
        {
            var product = Mock.Of<ISetupPackageReference>(x => x.GetId() == "product");

            var instance = new Mock<ISetupInstance2>();
            instance.Setup(x => x.GetInstanceId()).Returns("test");
            instance.Setup(x => x.GetProduct()).Returns(product);

            var sut = new Instance(instance.Object);
            Assert.NotNull(sut.Product);
            Assert.Equal("product", sut.Product.Id);
        }

        [Fact]
        public void New_Packages_Null()
        {
            var instance = new Mock<ISetupInstance2>();
            instance.Setup(x => x.GetInstanceId()).Returns("test");

            var sut = new Instance(instance.Object);
            Assert.Null(sut.Packages);
        }

        [Fact]
        public void New_Packages_Empty()
        {
            var instance = new Mock<ISetupInstance2>();
            instance.Setup(x => x.GetInstanceId()).Returns("test");
            instance.Setup(x => x.GetPackages()).Returns(Enumerable.Empty<ISetupPackageReference>().ToArray());

            var sut = new Instance(instance.Object);
            Assert.Null(sut.Packages);
        }

        [Fact]
        public void New_Packages()
        {
            var references = new[]
            {
                Mock.Of<ISetupPackageReference>(x => x.GetId() == "a"),
                Mock.Of<ISetupPackageReference>(x => x.GetId() == "b"),
            };

            var instance = new Mock<ISetupInstance2>();
            instance.Setup(x => x.GetInstanceId()).Returns("test");
            instance.Setup(x => x.GetPackages()).Returns(references);

            var sut = new Instance(instance.Object);
            Assert.Collection(
                sut.Packages,
                x => Assert.Equal("a", x.Id),
                x => Assert.Equal("b", x.Id));
        }
    }
}
